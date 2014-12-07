package com.cnblogs.android;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.cnblogs.android.core.Config;
import com.cnblogs.android.entity.BlogCategory;
import com.cnblogs.android.enums.EnumActivityType;
import com.cnblogs.android.utility.BlogListHtmlParse;
import com.cnblogs.android.utility.NetHelper;

import android.app.ListActivity;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;

public class BlogCategoryActivity extends BaseActivity implements
		OnItemClickListener {

	Resources res;
	ListView listview;
	ImageButton blog_category_ok;
	BlogCategory selectedCategory;
	List<BlogCategory> categories;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_blog_category);
		res = this.getResources();

		InitialControls();
	}

	/*
	 * ��ʼ���ؼ�
	 */
	void InitialControls() {
		Button btnBack = (Button) findViewById(R.id.btn_back);
		btnBack.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				finish();
			}
		});

		ImageButton blog_category_ok = (ImageButton) findViewById(R.id.blog_category_ok);
		blog_category_ok.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				/*Intent data = new Intent();			 
				data.putExtra(Config.SELECTED_BLOG_CAGEGORY, selectedCategory);
				setResult(Config.REQUEST_BLOG_CAGEGORY, data);
				finish();*/
			}
		});

		categories = getCategories();

		listview = (ListView) findViewById(R.id.category_list);
		listview.setAdapter(new CategoryAdapter(this, categories));
		// ���ü���
		listview.setOnItemClickListener(this);

		
		Log.e("NetHelper", "______________��ȡ����Over ");
	}

	// item�ĵ������ʱ��
	@Override
	public void onItemClick(AdapterView<?> view, View arg1, int position,
			long arg3) {
		selectedCategory = categories.get(position);
		Intent intent = new Intent();		 
		Bundle bundle = new Bundle();
		bundle.putSerializable(Config.SELECTED_BLOG_CAGEGORY, selectedCategory);
		intent.putExtras(bundle); 
		setResult(Config.REQUEST_BLOG_CAGEGORY, intent);
		BlogCategoryActivity.this.finish();
	}

	public List<BlogCategory> getCategories() {
		List<BlogCategory> categories = new ArrayList<BlogCategory>();
		categories.add(new BlogCategory("808","��ҳ","/","SiteHome","0"));
		categories.add(new BlogCategory("108698",".NET����","/cate/108698/","TopSiteCategory","0"));
		categories.add(new BlogCategory("18156",".NET������","/cate/beginner/","SiteCategory","108698"));
		categories.add(new BlogCategory("108699","ASP.NET","/cate/aspnet/","SiteCategory","108698"));
		categories.add(new BlogCategory("108700","C#","/cate/csharp/","SiteCategory","108698"));
		categories.add(new BlogCategory("108716","WinForm","/cate/winform/","SiteCategory","108698"));
		categories.add(new BlogCategory("108717","Silverlight","/cate/silverlight/","SiteCategory","108698"));
		categories.add(new BlogCategory("108718","WCF","/cate/wcf/","SiteCategory","108698"));
		categories.add(new BlogCategory("108719","CLR","/cate/clr/","SiteCategory","108698"));
		categories.add(new BlogCategory("108720","WPF","/cate/wpf/","SiteCategory","108698"));
		categories.add(new BlogCategory("108728","XNA","/cate/xna/","SiteCategory","108698"));
		categories.add(new BlogCategory("108729","Visual Studio","/cate/vs2010/","SiteCategory","108698"));
		categories.add(new BlogCategory("108730","ASP.NET MVC","/cate/mvc/","SiteCategory","108698"));
		categories.add(new BlogCategory("108738","�ؼ�����","/cate/control/","SiteCategory","108698"));
		categories.add(new BlogCategory("108739","Entity Framework","/cate/ef/","SiteCategory","108698"));
		categories.add(new BlogCategory("108745","WinRT/Metro","/cate/winrt_metro/","SiteCategory","108698"));
		categories.add(new BlogCategory("2","�������","/cate/2/","TopSiteCategory","0"));
		categories.add(new BlogCategory("106876","Java","/cate/java/","SiteCategory","2"));
		categories.add(new BlogCategory("106880","C++","/cate/cpp/","SiteCategory","2"));
		categories.add(new BlogCategory("106882","PHP","/cate/php/","SiteCategory","2"));
		categories.add(new BlogCategory("106877","Delphi","/cate/delphi/","SiteCategory","2"));
		categories.add(new BlogCategory("108696","Python","/cate/python/","SiteCategory","2"));
		categories.add(new BlogCategory("106894","Ruby","/cate/ruby/","SiteCategory","2"));
		categories.add(new BlogCategory("108735","C","/cate/c/","SiteCategory","2"));
		categories.add(new BlogCategory("108746","Erlang","/cate/erlang/","SiteCategory","2"));
		categories.add(new BlogCategory("108748","Go","/cate/go/","SiteCategory","2"));
		categories.add(new BlogCategory("108751","Swift","/cate/swift/","SiteCategory","2"));
		categories.add(new BlogCategory("108742","Verilog","/cate/verilog/","SiteCategory","2"));
		categories.add(new BlogCategory("108701","������","/cate/108701/","TopSiteCategory","0"));
		categories.add(new BlogCategory("106892","�ܹ����","/cate/design/","SiteCategory","108701"));
		categories.add(new BlogCategory("108702","�������","/cate/108702/","SiteCategory","108701"));
		categories.add(new BlogCategory("106884","���ģʽ","/cate/dp/","SiteCategory","108701"));
		categories.add(new BlogCategory("108750","�����������","/cate/ddd/","SiteCategory","108701"));
		categories.add(new BlogCategory("108703","Webǰ��","/cate/108703/","TopSiteCategory","0"));
		categories.add(new BlogCategory("106883","Html/Css","/cate/web/","SiteCategory","108703"));
		categories.add(new BlogCategory("106893","JavaScript","/cate/javascript/","SiteCategory","108703"));
		categories.add(new BlogCategory("108731","jQuery","/cate/jquery/","SiteCategory","108703"));
		categories.add(new BlogCategory("108737","HTML5","/cate/html5/","SiteCategory","108703"));
		categories.add(new BlogCategory("108704","��ҵ��Ϣ��","/cate/108704/","TopSiteCategory","0"));
		categories.add(new BlogCategory("78111","SharePoint","/cate/sharepoint/","SiteCategory","108704"));
		categories.add(new BlogCategory("50349","GIS����","/cate/gis/","SiteCategory","108704"));
		categories.add(new BlogCategory("106878","SAP","/cate/sap/","SiteCategory","108704"));
		categories.add(new BlogCategory("108732","Oracle ERP","/cate/OracleERP/","SiteCategory","108704"));
		categories.add(new BlogCategory("108734","Dynamics CRM","/cate/dynamics/","SiteCategory","108704"));
		categories.add(new BlogCategory("108747","K2 BPM","/cate/k2/","SiteCategory","108704"));
		categories.add(new BlogCategory("108749","��Ϣ��ȫ","/cate/infosec/","SiteCategory","108704"));
		categories.add(new BlogCategory("3","��ҵ��Ϣ������","/cate/3/","SiteCategory","108704"));
		categories.add(new BlogCategory("108705","�ֻ�����","/cate/108705/","TopSiteCategory","0"));
		categories.add(new BlogCategory("108706","Android����","/cate/android/","SiteCategory","108705"));
		categories.add(new BlogCategory("108707","iOS����","/cate/ios/","SiteCategory","108705"));
		categories.add(new BlogCategory("108736","Windows Phone","/cate/wp/","SiteCategory","108705"));
		categories.add(new BlogCategory("108708","Windows Mobile","/cate/wm/","SiteCategory","108705"));
		categories.add(new BlogCategory("106886","�����ֻ�����","/cate/mobile/","SiteCategory","108705"));
		categories.add(new BlogCategory("108709","�������","/cate/108709/","TopSiteCategory","0"));
		categories.add(new BlogCategory("108710","���ݿ���","/cate/agile/","SiteCategory","108709"));
		categories.add(new BlogCategory("106891","��Ŀ���Ŷӹ���","/cate/pm/","SiteCategory","108709"));
		categories.add(new BlogCategory("106889","�����������","/cate/Engineering/","SiteCategory","108709"));
		categories.add(new BlogCategory("108712","���ݿ⼼��","/cate/108712/","TopSiteCategory","0"));
		categories.add(new BlogCategory("108713","SQL Server","/cate/sqlserver/","SiteCategory","108712"));
		categories.add(new BlogCategory("108714","Oracle","/cate/oracle/","SiteCategory","108712"));
		categories.add(new BlogCategory("108715","MySQL","/cate/mysql/","SiteCategory","108712"));
		categories.add(new BlogCategory("108743","NoSQL","/cate/nosql/","SiteCategory","108712"));
		categories.add(new BlogCategory("106881","�������ݿ�","/cate/database/","SiteCategory","108712"));
		categories.add(new BlogCategory("108724","����ϵͳ","/cate/108724/","TopSiteCategory","0"));
		categories.add(new BlogCategory("108721","Windows 7","/cate/win7/","SiteCategory","108724"));
		categories.add(new BlogCategory("108725",">Windows Server","/cate/winserver/","SiteCategory","108724"));
		categories.add(new BlogCategory("108726","Linux","/cate/linux/","SiteCategory","108724"));
		categories.add(new BlogCategory("4","��������","/cate/4/","TopSiteCategory","0"));

		return categories;
	}

	
	public class CategoryAdapter extends BaseAdapter {

		private Context context;
		private List<BlogCategory> lists;
		private LayoutInflater layoutInflater;
		ImageView img;
		TextView tv1;
		ImageView imageIsSelected;

		/**
		 * ���캯�������г�ʼ��
		 * 
		 * @param context
		 * @param lists
		 */
		CategoryAdapter(Context context, List<BlogCategory> lists) {
			this.context = context;
			this.lists = lists;
			layoutInflater = LayoutInflater.from(this.context);
		}

		// ��ó��ȣ�һ�㷵�����ݵĳ��ȼ���
		@Override
		public int getCount() {
			return lists.size();
		}

		@Override
		public Object getItem(int position) {
			return lists.get(position);
		}

		@Override
		public long getItemId(int position) {
			return position;
		}

		/**
		 * ����Ҫ�ķ�����ÿһ��item���ɵ�ʱ�򣬶���ִ����������������������ʵ��������item��ÿ���ؼ��İ�
		 */
		@Override
		public View getView(int position, View convertView, ViewGroup parent) {

			if (convertView == null) {
				convertView = layoutInflater.inflate(
						R.layout.category_list_item, null);
			}

			img = (ImageView) convertView.findViewById(R.id.category_image);
			tv1 = (TextView) convertView.findViewById(R.id.categroy_text);
			imageIsSelected = (ImageView) convertView
					.findViewById(R.id.category_selected);

			// img.setBackgroundResource(lists.get(position).getPicture());
			tv1.setText(lists.get(position).Name);

			return convertView;
		}

	}
}
