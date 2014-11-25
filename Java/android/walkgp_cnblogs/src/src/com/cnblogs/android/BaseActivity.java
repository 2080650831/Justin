package com.cnblogs.android;
import com.cnblogs.android.utility.PreferencesHelper;
import android.app.Activity;
import android.content.Intent; 
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.view.KeyEvent;
import android.webkit.WebSettings.TextSize;

/**
 * ���࣬�󲿷�Activity�̳��Դ���
 * 
 * @author walkingp
 * @date:2011-11
 * 
 */
public class BaseActivity extends Activity {
 
	TextSize textSize = TextSize.SMALLEST;

	@Override
	protected void onResume() {
		super.onResume();
		if (!PreferencesHelper.getIsAutoHorizontal())
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		else
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_USER);
	}

	protected void onPause() {
		super.onPause();
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		textSize = PreferencesHelper.getStoredTextSize();
	}

	/**
	 * ���¼����Ϸ��ذ�ť
	 */
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_SEARCH) {// ����
			Intent intent = new Intent(BaseActivity.this, SearchActivity.class);
			intent.putExtra("isShowQuitHints", false);
			startActivity(intent);
			return true;
		} else {
			return super.onKeyDown(keyCode, event);
		}
	}
}
