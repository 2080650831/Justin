package com.cnblogs.android.utility;

import java.io.FileOutputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.cnblogs.android.BlogCategoryActivity.BlogCategory;
import com.cnblogs.android.entity.Blog;

public class BlogListHtmlParse {

	// �������������б�ʱ��Ҫ�Ĳ�����Ϣ
	public static BlogCategory JiexiCategoryInfo(String categoryInfoHtml,
			BlogCategory categoryInfo) {
		// var aggSiteModel =
		// {"CategoryType":"SiteCategory","ParentCategoryId":108712,"CategoryId":108713,"PageIndex":1,"ItemListActionName":"PostList"};
		Pattern categoryInfoPattern = Pattern
				.compile("\"CategoryType\":\"(\\w+)\",\"ParentCategoryId\":(\\d+),\"CategoryId\":(\\d+),");
		Matcher categoryInfoMatcher = categoryInfoPattern
				.matcher(categoryInfoHtml);
		while (categoryInfoMatcher.find()) {

			categoryInfo.CategoryType = categoryInfoMatcher.group(1);
			categoryInfo.ParentCategoryId = categoryInfoMatcher.group(2);
			categoryInfo.CategoryId = categoryInfoMatcher.group(3);
		}
		return categoryInfo;
	}

	public static Blog jiexiBlog(String blog_html) {

		Blog blog = new Blog();
		try {
			String blog_regex = "digg_count_(\\d*)\">(\\d*)</span>[\\s\\S]+?href=\"(.*)\"\\s+target=\"_blank\">([\\s\\S]+?)</a></h3>[\\s\\S]+?>([\\s\\S]+?)</p>[\\s\\S]+?href=\"(.*?)\"[\\s\\S]+?\">(.*)</a>([\\s\\S]+?)<span[\\s\\S]+?\\((.*)\\)[\\s\\S]+?\\((.*)\\)";

			Pattern blog_pattern = Pattern.compile(blog_regex);
			Matcher blog_pattern_matcher = blog_pattern.matcher(blog_html);
			while (blog_pattern_matcher.find()) {
				String blogid = blog_pattern_matcher.group(1);//
				String diggnum = blog_pattern_matcher.group(2);//
				String blogurl = blog_pattern_matcher.group(3);
				String blogtitle = blog_pattern_matcher.group(4);
				String blogsummary = blog_pattern_matcher.group(5);//
				String blogerurl = blog_pattern_matcher.group(6);
				String blogername = blog_pattern_matcher.group(7);
				String posttime = blog_pattern_matcher.group(8)
						.replace("������", "").replace(" ", "");
				String comment = blog_pattern_matcher.group(9);//
				String read = blog_pattern_matcher.group(10);//

				// blog.SetAddTime(addTime)
				blog.SetAuthor(blogername);
				blog.SetAuthorUrl(blogerurl);
				// blog.SetAvator(avator);
				// blog.SetBlogContent(content);
				blog.SetBlogId(Integer.valueOf(blogid));
				blog.SetBlogTitle(blogtitle);
				blog.SetBlogUrl(blogurl);
				// blog.SetCateId(_cateId);
				// blog.SetCateName(_cateName);
				blog.SetCommentNum(Integer.valueOf(comment));
				blog.SetDiggsNum(Integer.valueOf(diggnum));
				// blog.SetIsFullText(_isFullText);
				// blog.SetIsReaded(_isReaded);
				blog.SetSummary(blogsummary);
				// blog.SetUpdateTime(updateTime);
				// blog.SetUserName(userName);
				blog.SetViewNum(Integer.valueOf(read));
			}

			Matcher imageMatcher = Pattern.compile(
					"<img[\\s\\S]+?src=\"(.*?)\"").matcher(blog_html);
			if (imageMatcher.find()) {
				blog.SetAvator(imageMatcher.group(1));
			}

		} catch (Exception ex) {

		}
		return blog;
	}

	public static List<Blog> JiexiBlogList(String blog_list_html) {

		List<Blog> blogs = new ArrayList<Blog>();
		String blog_list_regex = "class=\"post_item\">[\\s\\S]+?class=\"post_item_body\"[\\s\\S]+?class=\"post_item_foot\"[\\s\\S]+?class=\"clear\"";
		Pattern blog_list_pattern = Pattern.compile(blog_list_regex);
		Matcher blog_list_pattern_matcher = blog_list_pattern
				.matcher(blog_list_html);
		while (blog_list_pattern_matcher.find()) {
			String blog_html = blog_list_pattern_matcher.group();
			Blog blog = jiexiBlog(blog_html);
			if (blog != null) {
				blogs.add(blog);
			}

		}
		return blogs;
	}

	public static void writeFileSdcard(String fileName, String message) {

		try {

			// FileOutputStream fout = openFileOutput(fileName, MODE_PRIVATE);

			FileOutputStream fout = new FileOutputStream(fileName);

			byte[] bytes = message.getBytes();

			fout.write(bytes);

			fout.close();

		}

		catch (Exception e) {

			e.printStackTrace();

		}

	}

	/*class="post_item">
	<div class="digg">
	    <div class="diggit" onclick="DiggPost('insus',4072508,31588,1)"> 
		<span class="diggnum" id="digg_count_4072508">1</span>
		</div>
		<div class="clear"></div>
		<div id="digg_tip_4072508" class="digg_tip"></div>
	</div>      
	<div class="post_item_body">
		<h3><a class="titlelnk" href="http://www.cnblogs.com/insus/p/4072508.html" target="_blank">����λ�������ʹ��$���Ų���</a></h3>               	
	    <p class="post_item_summary">
	    ��Ҫ�����ַ�������Ҫ�󳤶�Ϊ5���ַ����������λ���������ȣ���ǰ�棨��ߣ�ʹ��&quot;$&quot;���Ų��㡣��ʵ������⣬ʵ���������ѣ���ΪC#�����У��ʹ��д˹��ܣ�����PadLeft()����������Insus.NET����ҳ�У��оټ���������˵����������Ҫȥ��ȡLabel+ �����ı�ǩֵ��������ϣ�ȥ��ʾ��Labe... 
	    </p>              
	    <div class="post_item_foot">                    
	    <a href="http://www.cnblogs.com/insus/" class="lightblue">Insus.NET</a> 
	    ������ 2014-11-04 08:07 
	    <span class="article_comment"><a href="http://www.cnblogs.com/insus/p/4072508.html#commentform" title="" class="gray">
	        ����(0)</a></span><span class="article_view"><a href="http://www.cnblogs.com/insus/p/4072508.html" class="gray">�Ķ�(130)</a></span></div>
	</div>
	<div class="clear"
	------------------------------
	diggnum:1
	blogurl:http://www.cnblogs.com/insus/p/4072508.html
	blogtitle:����λ�������ʹ��$���Ų���
	blogsummary:
	    ��Ҫ�����ַ�������Ҫ�󳤶�Ϊ5���ַ����������λ���������ȣ���ǰ�棨��ߣ�ʹ��&quot;$&quot;���Ų��㡣��ʵ������⣬ʵ���������ѣ���ΪC#�����У��ʹ��д˹��ܣ�����PadLeft()����������Insus.NET����ҳ�У��оټ���������˵����������Ҫȥ��ȡLabel+ �����ı�ǩֵ��������ϣ�ȥ��ʾ��Labe... 
	    
	blogerurl:http://www.cnblogs.com/insus/
	blogername:Insus.NET
	posttime: 
	    ������ 2014-11-04 08:07 
	    
	comment:0
	read:130*/
}
