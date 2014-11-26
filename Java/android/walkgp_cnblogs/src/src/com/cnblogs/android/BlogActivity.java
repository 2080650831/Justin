package com.cnblogs.android;

import java.util.ArrayList;
import java.util.List;

import android.annotation.SuppressLint;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.res.Resources;
import android.graphics.Color;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.LayoutInflater;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnCreateContextMenuListener;
import android.widget.AdapterView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AbsListView;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.cnblogs.android.adapter.BlogListAdapter;
import com.cnblogs.android.adapter.BlogListAdapter.BlogViewHolder;
import com.cnblogs.android.core.BlogHelper;
import com.cnblogs.android.core.Config;
import com.cnblogs.android.entity.Blog;
import com.cnblogs.android.utility.NetHelper;
import com.cnblogs.android.controls.PullToRefreshListView;
import com.cnblogs.android.controls.PullToRefreshListView.OnRefreshListener;
import com.cnblogs.android.dal.BlogDalHelper;

/**
 * �����б�
 * 
 * @author walkingp
 * @date:2011-12
 * 
 */
public class BlogActivity extends BaseMainActivity {

	int currentPageIndex = 1;// ҳ��

	ListView listView;
	private BlogListAdapter adapter;// ����Դ
	List<Blog> listBlog = new ArrayList<Blog>();
	ProgressBar blogBody_progressBar;// ����ListView���ؿ�

	ImageButton blog_refresh_btn;// ˢ�°�ť
	ProgressBar blog_progress_bar;// ���ذ�ť

	private LinearLayout viewFooter;// footer view

	Resources res;// ��Դ
	private int lastItem;
	BlogDalHelper dbHelper;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		this.setContentView(R.layout.blog_layout);

		res = this.getResources();
		InitialControls();
		BindControls();
		new PageTask(0).execute();

		// ע��㲥
		UpdateListViewReceiver receiver = new UpdateListViewReceiver();
		IntentFilter filter = new IntentFilter();
		filter.addAction("android.cnblogs.com.update_bloglist");
		registerReceiver(receiver, filter);
	}

	// �����˵���Ӧ����
	@Override
	public boolean onContextItemSelected(MenuItem item) {

		AdapterView.AdapterContextMenuInfo menuInfo = (AdapterView.AdapterContextMenuInfo) item
				.getMenuInfo();
		Blog blog = getBlogByViewHolderContainer(menuInfo.targetView);

		if (blog == null)
			return false;

		int itemIndex = item.getItemId();

		switch (itemIndex) {
		case R.id.menu_blog_view:// ��ϸ
			RedirectDetailActivity(blog);
			break;
		case R.id.menu_blog_comment:// ����
			RedirectCommentActivity(blog);
			break;
		case R.id.menu_blog_author:// �����������
			RedirectAuthorActivity(blog);
			break;
		case R.id.menu_blog_browser:// ��������в鿴
			ViewInBrowser(blog);
			break;
		case R.id.menu_blog_share:// ����
			ShareTo(blog);
			break;
		}

		return super.onContextItemSelected(item);
	}

	/**
	 * ��ʼ���б�
	 */
	private void InitialControls() {
		listView = (ListView) findViewById(R.id.blog_list);
		blogBody_progressBar = (ProgressBar) findViewById(R.id.blogList_progressBar);
		blogBody_progressBar.setVisibility(View.VISIBLE);
		// ����������
		blog_refresh_btn = (ImageButton) findViewById(R.id.blog_refresh_btn);
		blog_progress_bar = (ProgressBar) findViewById(R.id.blog_progressBar);
		// �ײ�view
		LayoutInflater mInflater = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		viewFooter = (LinearLayout) mInflater.inflate(R.layout.listview_footer,
				null, false);
		dbHelper = new BlogDalHelper(getApplicationContext());
	}

	/**
	 * ���¼�
	 */
	private void BindControls() {
		// ˢ��
		blog_refresh_btn.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				new PageTask(-2).execute();
			}
		});
		// ����ˢ��
		((PullToRefreshListView) listView)
				.setOnRefreshListener(new OnRefreshListener() {
					@Override
					public void onRefresh() {
						new PageTask(-1).execute();
					}
				});
		// �������ظ���
		listView.setOnScrollListener(new OnScrollListener() {
			/**
			 * ���������һ��
			 */
			@Override
			public void onScrollStateChanged(AbsListView view, int scrollState) {
				if (lastItem >= adapter.getCount()
						&& scrollState == OnScrollListener.SCROLL_STATE_IDLE) {
					new PageTask(currentPageIndex + 1).execute();
				}
			}

			@Override
			public void onScroll(AbsListView view, int firstVisibleItem,
					int visibleItemCount, int totalItemCount) {
				lastItem = firstVisibleItem - 2 + visibleItemCount;
			}
		});
		// �����ת
		listView.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View v,
					int position, long id) {
				Blog blog = getBlogByViewHolderContainer(v);
				if (blog != null)
					RedirectDetailActivity(blog);
			}
		});
		// �����¼�
		listView.setOnCreateContextMenuListener(new OnCreateContextMenuListener() {
			@Override
			public void onCreateContextMenu(ContextMenu menu, View v,
					ContextMenuInfo menuInfo) {
				MenuInflater inflater = getMenuInflater();
				inflater.inflate(R.menu.blog_list_contextmenu, menu);
				menu.setHeaderTitle(R.string.menu_bar_title);
			}
		});
	}

	/**
	 * ��ת������
	 * 
	 * @param v
	 */
	private void RedirectCommentActivity(Blog blog) {

		if (blog.GetCommentNum() == 0) {
			Toast.makeText(getApplicationContext(), R.string.sys_empty_comment,
					Toast.LENGTH_SHORT).show();
			// return;
		}
		Intent intent = new Intent();
		intent.setClass(BlogActivity.this, CommentActivity.class);
		Bundle bundle = new Bundle();
		bundle.putInt("contentId", blog.GetBlogId());
		bundle.putInt("commentType", 0);
		bundle.putString("title", blog.GetBlogTitle());
		bundle.putString("url", blog.GetBlogUrl());
		intent.putExtras(bundle);

		startActivity(intent);
	}

	/**
	 * ��ת������
	 * 
	 * @param v
	 */
	private void RedirectDetailActivity(Blog blog) {
		try {
			Intent intent = new Intent();
			intent.setClass(BlogActivity.this, BlogDetailActivity.class);
			Bundle bundle = new Bundle();
			bundle.putSerializable("blog", blog);
			intent.putExtras(bundle);
			startActivity(intent);

		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}

	/**
	 * ��������в鿴
	 * 
	 * @param v
	 */
	private void ViewInBrowser(Blog blog) {
		Uri blogUri = Uri.parse(blog.GetBlogUrl());
		Intent it = new Intent(Intent.ACTION_VIEW, blogUri);
		startActivity(it);
	}

	/**
	 * ��ת�������������
	 * 
	 * @param v
	 */
	private void RedirectAuthorActivity(Blog blog) {

		if (blog.GetUserName().equals("")) {
			Toast.makeText(getApplicationContext(), R.string.sys_no_author,
					Toast.LENGTH_SHORT).show();
			return;
		}

		Intent intent = new Intent();
		intent.setClass(BlogActivity.this, AuthorBlogActivity.class);
		Bundle bundle = new Bundle();
		bundle.putString("author", blog.GetUserName());
		bundle.putString("blogName", blog.GetAuthor());

		intent.putExtras(bundle);

		startActivity(intent);
	}

	/**
	 * ����
	 * 
	 * @param v
	 */
	private void ShareTo(Blog blog) {

		String shareContent = "��" + blog.GetBlogTitle() + "��,���ߣ�"
				+ blog.GetAuthor() + "��ԭ�����ӣ�" + blog.GetBlogUrl() + " �����ԣ�"
				+ res.getString(R.string.app_name) + "Android�ͻ���("
				+ res.getString(R.string.app_homepage) + ")";

		Intent intent = new Intent(Intent.ACTION_SEND);
		intent.setType("text/plain");
		intent.putExtra(Intent.EXTRA_SUBJECT, "��ѡ�������");
		intent.putExtra(Intent.EXTRA_TEXT, shareContent);
		startActivity(Intent.createChooser(intent, blog.GetBlogTitle()));
	}

	public Blog getBlogByViewHolderContainer(View v) {
		Object tag = v.getTag();
		Blog blog = null;
		if (tag != null && tag instanceof BlogViewHolder) {
			BlogViewHolder blogView = (BlogViewHolder) tag;
			blog = blogView.blog;
			if (blog == null) {
				Toast.makeText(getApplicationContext(), "�Ҳ�����Blog",
						Toast.LENGTH_SHORT).show();
			}
		}
		return blog;
	}

	/**
	 * ����ListViewΪ�Ѷ�״̬ �˹㲥ͬʱ��BlogDeatail��DownloadServices
	 * 
	 * @author walkingp
	 * 
	 */
	public class UpdateListViewReceiver extends BroadcastReceiver {
	 
		@Override
		public void onReceive(Context content, Intent intent) {

			Bundle bundle = intent.getExtras();
			int[] blogIdArr = bundle.getIntArray("blogIdArray");
			for (int i = 0, len = listView.getChildCount(); i < len; i++) {
				View view = listView.getChildAt(i);
				TextView tvId = (TextView) view
						.findViewById(R.id.recommend_text_id);
				if (tvId != null) {

					int blogId = Integer.parseInt(tvId.getText().toString());

					ImageView icoDown = (ImageView) view
							.findViewById(R.id.icon_downloaded);
					TextView tvTitle = (TextView) view
							.findViewById(R.id.recommend_text_title);

					for (int j = 0, size = blogIdArr.length; j < size; j++) {
						if (blogId == blogIdArr[j]) {
							icoDown.setVisibility(View.VISIBLE);// �Ѿ�����
							tvTitle.setTextColor(Color.BLUE);// �Ѷ�
						}
					}

				}
			}
			for (int i = 0, len = blogIdArr.length; i < len; i++) {
				for (int j = 0, size = listBlog.size(); j < size; j++) {
					if (blogIdArr[i] == listBlog.get(j).GetBlogId()) {
						listBlog.get(i).SetIsFullText(true);
						listBlog.get(i).SetIsReaded(true);
					}
				}
			}
		}
	}

	/**
	 * ���߳������������������ء���ʼ�������ؼ��ء�ˢ�£�
	 * 
	 */
	public class PageTask extends AsyncTask<String, Integer, List<Blog>> {
		int curPageIndex = 0;
		boolean isDataFromLocal = false;// �Ƿ��Ǵӱ��ض�ȡ������

		int requestPageIndex = 0;

		public PageTask(int page) {
			curPageIndex = page;
		}

		protected List<Blog> doInBackground(String... params) {
			boolean isNetworkAvailable = NetHelper
					.networkIsAvailable(getApplicationContext());

			requestPageIndex = curPageIndex;
			if (requestPageIndex <= 0) {
				requestPageIndex = 1;
			}
			if (curPageIndex == -2) {
				requestPageIndex = currentPageIndex;
			}

			// ���ȶ�ȡ��������
			List<Blog> listBlogLocal = dbHelper.GetBlogListByPage(
					requestPageIndex, Config.BLOG_PAGE_SIZE);
			List<Blog> listBlogNet = new ArrayList<Blog>();
			if (isNetworkAvailable) {
				listBlogNet.addAll(BlogHelper.GetBlogList(requestPageIndex));
			}

			List<Blog> newBlogs = isNetworkAvailable ? listBlogNet
					: listBlogLocal;
			isDataFromLocal = isNetworkAvailable ? false : true;

			List<Blog> additionalBlogs = new ArrayList<Blog>();

			for (Blog blog : newBlogs) {
				if (!listBlog.contains(blog)) {
					additionalBlogs.add(blog);
				}
			}

			switch (curPageIndex) {
			case -2:// ˢ�µ�ǰ����ҳ���� ��������������б�β��
				return additionalBlogs;
			case -1:// ����ˢ�� ���������ݲ����б�
				return additionalBlogs;
			case 0:// �״μ��� �б�Clear�󣬽�������������б�
				if (isNetworkAvailable) {
					isDataFromLocal = false;
					return listBlogNet;
				} else {
					isDataFromLocal = true;
					return listBlogLocal;
				}

			default:// ���ظ������� ��������������б�β�� ��ǰҳ��1
				return additionalBlogs;
			}
		}

		@Override
		protected void onCancelled() {
			super.onCancelled();
		}

		/**
		 * ��������
		 */
		/*
		 * (non-Javadoc)
		 * 
		 * @see android.os.AsyncTask#onPostExecute(java.lang.Object)
		 */
		@Override
		protected void onPostExecute(List<Blog> result) {

			/*	Toast.makeText(getApplicationContext(),
						"���ص�" + String.valueOf(requestPageIndex) + "ҳ",
						Toast.LENGTH_SHORT).show();*/
			// ���Ͻ�
			blogBody_progressBar.setVisibility(View.GONE);
			blog_progress_bar.setVisibility(View.GONE);
			blog_refresh_btn.setVisibility(View.VISIBLE);

			if (result == null || result.size() == 0) {// û��������
				((PullToRefreshListView) listView).onRefreshComplete();
				boolean isNetworkAvailable = NetHelper
						.networkIsAvailable(getApplicationContext());
				String tips = "";
				if (curPageIndex > 1) {
					tips = "�Ѿ�û�и��������ˡ�";
				} else if (curPageIndex == -1) {
					tips = "����޸��¡�";
				} else if (curPageIndex == 0) {
					tips = isNetworkAvailable ? "�����޻��档" : "��������";
				}
				if (tips.length() != 0)
					Toast.makeText(getApplicationContext(), tips,
							Toast.LENGTH_SHORT).show();
				return;
			}
			int size = result.size();
			if (size >= Config.BLOG_PAGE_SIZE
					&& listView.getFooterViewsCount() == 0) {
				listView.addFooterView(viewFooter);
			}
			// ���浽���ݿ�
			if (!isDataFromLocal) {
				dbHelper.SynchronyData2DB(result);
			}
			if (adapter == null) {
				adapter = new BlogListAdapter(getApplicationContext(),
						listBlog, listView);
				listView.setAdapter(adapter);
				((PullToRefreshListView) listView)
						.SetPageSize(Config.BLOG_PAGE_SIZE);
			}

			boolean refresh = true;
			switch (curPageIndex) {
			case -2:// ˢ�µ�ǰҳ
				adapter.AddMoreData(result);
				refresh = false;
				break;
			case -1:// �������µ�
				adapter.InsertData(result);
				break;
			case 0:// �״μ���
				adapter.GetData().clear();
				adapter.AddMoreData(result);
				break;
			default:// ������һҳ
				adapter.AddMoreData(result);
				currentPageIndex = currentPageIndex + 1;
				refresh = false;
				break;
			}
			((PullToRefreshListView) listView).SetDataRow(adapter.GetData()
					.size());
			if (refresh)
				((PullToRefreshListView) listView).onRefreshComplete();
		}

		@Override
		protected void onPreExecute() {
			// ���������
			if (listView.getCount() == 0) {
				blogBody_progressBar.setVisibility(View.VISIBLE);
			}
			// ���Ͻ�
			blog_progress_bar.setVisibility(View.VISIBLE);
			blog_refresh_btn.setVisibility(View.GONE);

		}

		@Override
		protected void onProgressUpdate(Integer... values) {
		}
	}
}
