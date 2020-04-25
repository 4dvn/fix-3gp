// FIX_DJI.Form1
using FIX_DJI;
using FIX_DJI.Properties;
using MediaInfoDotNet;
using SlavaGu.ConsoleAppLauncher;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class Form1 : Form
{
	private static int FASE = 1;

	private string vbad = "";

	private string vgood = "";

	private string vfixed = "";

	private string vh264 = "Fixed.h264";

	private string vfixedmp4 = "";

	private string vtest = "";

	private const string sfixer = "fixer.exe";

	private const string fixer = "recover";

	private const string sffmpeg = "ffmpeg.exe";

	private const string ffmpeg = "ffmpeg";

	private const string sdjifix = "videoreco.exe";

	private const string djifix = "videoreco";

	private string djifix_type = "1";

	private const string reg = "0[xX][0-9a-fA-F]+";

	private string logresponse;

	private string path;

	private bool ERR;

	private string FR_vh264 = "30";

	private IContainer components;

	private Button button1;

	private TextBox textBox1;

	private TextBox textBox2;

	private Button button2;

	private Button button3;

	private TextBox textBox3;

	private ProgressBar progressBar1;

	private Label label1;

	private Label label2;

	private PictureBox pictureBox1;

	private Label label4;

	private TextBox tbfase1;

	private TextBox tbfase2;

	private TextBox tbfase3;

	private Label label3;

	private CheckBox checkBox1;

	private Label label5;

	private ComboBox textBox1c;

	private Button button5;

	private Label label6;

	private Label lbvbad;

	private Label lbvgood;

	private Label lbckvgood;

	private Label lbckvbad;

	private CheckBox checkBox2;

	private TrackBar trackBar1;

	private Label label9;

	private Label label7;

	private Label label8;

	public Form1()
	{
		InitializeComponent();
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		Text = Text + " " + Application.ProductVersion;
		textBox1c.SelectedIndex = 0;
		ExtractResource("ConsoleAppLauncher", "ConsoleAppLauncher.dll");
		ExtractResource("MediaInfo", "MediaInfo.dll");
		ExtractResource("MediaInfoDotNet", "MediaInfoDotNet.dll");
	}

	private void textBox3_TextChanged_1(object sender, EventArgs e)
	{
		textBox3.Select(textBox3.TextLength, 0);
		textBox3.ScrollToCaret();
	}

	private void textBox3_DoubleClick(object sender, EventArgs e)
	{
		if (File.Exists(vtest))
		{
			Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = vtest;
			process.Start();
		}
	}

	private void textBox1c_TextChanged(object sender, EventArgs e)
	{
		successreset(0);
		if (textBox1c.SelectedItem != textBox1c.Items[0])
		{
			textBox1c.Items[0] = "Browse for a video File or choose your settings --->";
			button1.Enabled = false;
		}
		else
		{
			button1.Enabled = true;
		}
		lbckvgood.Text = "";
		lbckvgood.Text = "";
		checkbuttons();
	}

	public void dofix(string exe, string url, Action<string> replyHandler)
	{
		ConsoleApp consoleApp = new ConsoleApp(exe, url);
		consoleApp.Exited += app_Exited;
		consoleApp.ConsoleOutput += delegate(object sender, ConsoleOutputEventArgs e)
		{
			replyHandler(e.Line);
		};
		consoleApp.Run();
	}

	private void app_Exited(object sender, EventArgs e)
	{
		DoFlow();
	}

	private void DoFlow()
	{
		if (ERR)
		{
			failreset(FASE);
			path = "Log_Error.txt";
			createlog(path, logresponse);
			return;
		}
		switch (FASE)
		{
		case 99:
			successreset(99);
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "All Phases Finished";
				tbfase3.BackColor = Color.Green;
				tbfase3.ForeColor = Color.White;
			});
			break;
		case 22:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.BackColor = SystemColors.Control;
				tbfase2.Text = "DjiFix Running...";
				tbfase2.ForeColor = Color.Red;
			});
			ERR = true;
			taskA();
			break;
		case 0:
			successreset(0);
			break;
		case 1:
			Invoke((MethodInvoker)delegate
			{
				tbfase1.BackColor = SystemColors.Control;
				tbfase1.Text = "Phase 1 Running...";
				tbfase1.ForeColor = Color.Red;
			});
			ERR = true;
			task1();
			break;
		case 2:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.BackColor = SystemColors.Control;
				tbfase2.Text = "Phase 2 Running...";
				tbfase2.ForeColor = Color.Red;
			});
			ERR = true;
			task2();
			break;
		case 3:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.BackColor = SystemColors.Control;
				tbfase3.Text = "Transcoding to Mov...";
				tbfase3.ForeColor = Color.Red;
			});
			ERR = true;
			task3();
			break;
		case 4:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.BackColor = SystemColors.Control;
				tbfase3.Text = "Converting to mp4...";
				tbfase3.ForeColor = Color.Red;
			});
			ERR = true;
			task4();
			break;
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		successreset(0);
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "mp4 files (*.mp4)|*.mp4|mov files (*.mov)|*.mov|All files (*.*)|*.*"
		};
		DialogResult dialogResult = openFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			textBox1.Text = openFileDialog.FileName;
			textBox1c.Items[0] = openFileDialog.FileName;
			textBox1c.SelectedItem = textBox1c.Items[0];
			vgood = textBox1.Text;
			infogood(textBox1.Text);
		}
		checkbuttons();
	}

	private void button2_Click(object sender, EventArgs e)
	{
		successreset(0);
		string text = "h264 files (*.h264)|*.h264|mp4 files (*.mp4)|*.mp4|mov files (*.mov)|*.mov|All files (*.*)|*.*";
		string text2 = "mp4 files (*.mp4)|*.mp4|mov files (*.mov)|*.mov|h264 files (*.h264)|*.h264|All files (*.*)|*.*";
		string text3 = "mov files (*.mov)|*.mov|mp4 files (*.mp4)|*.mp4|h264 files (*.h264)|*.h264|All files (*.*)|*.*";
		string filter = text;
		if (textBox1.Text != string.Empty)
		{
			filter = ((Path.GetExtension(vgood).ToUpper() == ".MP4") ? text2 : text3);
		}
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = filter
		};
		DialogResult dialogResult = openFileDialog.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			textBox2.Text = openFileDialog.FileName;
			vfixed = Path.GetFileNameWithoutExtension(textBox2.Text) + "-Fixed.mov";
			infobad(textBox2.Text);
			string a = Path.GetExtension(textBox2.Text).ToLower();
			if (a == ".h264" && lbvbad.Text != "")
			{
				vh264 = textBox2.Text;
			}
			if (a != ".h264")
			{
				vh264 = "Fixed.h264";
				vbad = textBox2.Text;
			}
		}
		checkbuttons();
	}

	private void button3_Click(object sender, EventArgs e)
	{
		_ = textBox1c.Text;
		int selectedIndex = textBox1c.SelectedIndex;
		if (selectedIndex > 0 && checkfile(vbad))
		{
			djifix_type = getdjifix_type(selectedIndex);
			FASE = 22;
			DoFlow();
		}
		else if (checkfile(vgood) && checkfile(vbad))
		{
			FASE = 1;
			DoFlow();
		}
		else
		{
			MessageBox.Show("Files are Missing");
		}
	}

	private void button5_Click(object sender, EventArgs e)
	{
		string a = Path.GetExtension(textBox2.Text).ToLower();
		if (a == ".h264")
		{
			vh264 = textBox2.Text;
			FASE = 3;
			DoFlow();
		}
	}

	private void checkbuttons()
	{
		Invoke((MethodInvoker)delegate
		{
			bool flag = lbckvbad.Text != "" && File.Exists(textBox2.Text) && Path.GetExtension(textBox2.Text).ToLower() == ".h264";
			button5.Enabled = flag;
			bool flag2 = (textBox1c.SelectedIndex > 0 && File.Exists(textBox2.Text)) || (lbckvgood.Text != "" && File.Exists(textBox1c.Text));
			button3.Enabled = (flag ? (!flag) : flag2);
		});
	}

	private void disablebuttons()
	{
		Invoke((MethodInvoker)delegate
		{
			button1.Enabled = false;
			button2.Enabled = false;
			button3.Enabled = false;
			button5.Enabled = false;
			checkBox1.Enabled = false;
			checkBox2.Enabled = false;
			textBox3.Enabled = false;
			textBox3.Cursor = Cursors.WaitCursor;
			Cursor = Cursors.WaitCursor;
			trackBar1.Cursor = Cursors.Default;
		});
	}

	private void enablebuttons()
	{
		checkbuttons();
		Invoke((MethodInvoker)delegate
		{
			button1.Enabled = true;
			button2.Enabled = true;
			checkBox1.Enabled = true;
			checkBox2.Enabled = true;
			textBox3.Enabled = true;
			textBox3.Cursor = Cursors.Default;
			Cursor = Cursors.Default;
		});
	}

	private void successreset(int ph)
	{
		switch (ph)
		{
		case 99:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "All Phases Finished";
				tbfase3.BackColor = Color.Green;
				tbfase3.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		case 22:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.Text = "DjiFix Finished...";
				tbfase2.BackColor = Color.Green;
				tbfase2.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		case 0:
			Invoke((MethodInvoker)delegate
			{
				tbfase1.Text = "Phase 1 Running...";
				tbfase1.BackColor = SystemColors.Control;
				tbfase1.ForeColor = SystemColors.Control;
				tbfase2.Text = "Phase 2 Running...";
				tbfase2.BackColor = SystemColors.Control;
				tbfase2.ForeColor = SystemColors.Control;
				tbfase3.Text = "Phase 3 Running...";
				tbfase3.BackColor = SystemColors.Control;
				tbfase3.ForeColor = SystemColors.Control;
				progressBar1.Value = 0;
			});
			enablebuttons();
			break;
		case 1:
			Invoke((MethodInvoker)delegate
			{
				tbfase1.Text = "Phase 1 Finished!";
				tbfase1.BackColor = Color.Green;
				tbfase1.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		case 2:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.Text = "Phase 2 Finished...";
				tbfase2.BackColor = Color.Green;
				tbfase2.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		case 3:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "Phase 3 Finished...";
				tbfase3.BackColor = Color.Green;
				tbfase3.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		case 4:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "Phase 4 Finished...";
				tbfase3.BackColor = Color.Green;
				tbfase3.ForeColor = Color.White;
			});
			enablebuttons();
			break;
		}
	}

	private void failreset(int ph)
	{
		Invoke((MethodInvoker)delegate
		{
			tbfase1.Text = "Phase 1 Running...";
			tbfase1.BackColor = SystemColors.Control;
			tbfase1.ForeColor = SystemColors.Control;
			tbfase2.Text = "Phase 2 Running...";
			tbfase2.BackColor = SystemColors.Control;
			tbfase2.ForeColor = SystemColors.Control;
			tbfase3.Text = "Phase 3 Running...";
			tbfase3.BackColor = SystemColors.Control;
			tbfase3.ForeColor = SystemColors.Control;
		});
		enablebuttons();
		writestatus("Process Failed, Try again!!");
		switch (ph)
		{
		case 22:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.Text = "DjiFix Failed!";
				tbfase2.BackColor = Color.Red;
				tbfase2.ForeColor = Color.White;
			});
			break;
		case 1:
			Invoke((MethodInvoker)delegate
			{
				tbfase1.Text = "Phase 1 Failed!";
				tbfase1.BackColor = Color.Red;
				tbfase1.ForeColor = Color.White;
			});
			break;
		case 2:
			Invoke((MethodInvoker)delegate
			{
				tbfase2.Text = "Phase 2 Failed!";
				tbfase2.BackColor = Color.Red;
				tbfase2.ForeColor = Color.White;
			});
			break;
		case 3:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "Phase 3 failed!";
				tbfase3.BackColor = Color.Red;
				tbfase3.ForeColor = Color.White;
			});
			break;
		case 4:
			Invoke((MethodInvoker)delegate
			{
				tbfase3.Text = "Phase 4 Failed!";
				tbfase3.BackColor = Color.Red;
				tbfase3.ForeColor = Color.White;
			});
			break;
		}
	}

	private void taskA()
	{
		disablebuttons();
		ExtractResource("videoreco", "videoreco.exe");
		if (checkfile("videoreco.exe") & checkfile(vbad))
		{
			logresponse = startlog();
			string url = $"\"{vbad}\" {djifix_type}";
			writestatus(string.Format(Environment.NewLine + "Processing File: {0}...", vbad));
			writestatus("Please Wait, if the file is too big this may take some time. please be patient...");
			Invoke((MethodInvoker)delegate
			{
				progressBar1.Value = 0;
				progressBar1.Style = ProgressBarStyle.Marquee;
			});
			dofix("videoreco.exe", url, delegate(string reply)
			{
				writestatus(reply);
				logresponse = logresponse + reply + Environment.NewLine;
				if (reply.Contains("Repaired file is"))
				{
					Form1 form = this;
					string djifixfile = reply.Replace("Repaired file is ", "");
					djifixfile = djifixfile.Replace("\"", "");
					if (File.Exists(djifixfile))
					{
						vtest = djifixfile;
						vh264 = vtest;
						writestatus("File has been created!");
						writestatus("if a h264 was created browse to its location and hit 'Transcode Video'");
						successreset(22);
						Invoke((MethodInvoker)delegate
						{
							form.label1.Text = "Fixing 100% Complete!";
							form.textBox2.Text = djifixfile;
							form.progressBar1.Style = ProgressBarStyle.Blocks;
							form.progressBar1.Value = 100;
							form.infobad(djifixfile);
							form.checkbuttons();
							if (form.checkBox2.Checked && Path.GetExtension(form.vtest).ToLower() == ".h264")
							{
								FASE = 3;
							}
							else
							{
								FASE = 99;
							}
						});
					}
					path = "Log_PhA.txt";
					createlog(path, logresponse);
					successreset(2);
					ERR = false;
				}
			});
		}
		else
		{
			MessageBox.Show("Choose a video to Fix");
			enablebuttons();
		}
	}

	private void task4()
	{
		disablebuttons();
		Invoke((MethodInvoker)delegate
		{
			progressBar1.Value = 0;
			progressBar1.Style = ProgressBarStyle.Marquee;
		});
		string text = Path.Combine(Application.StartupPath.ToString(), "ffmpeg.exe");
		ExtractResource("ffmpeg", text);
		vfixedmp4 = Path.GetFileNameWithoutExtension(vfixed) + ".mp4";
		string text2 = "";
		text2 = $"-i {vfixed} -vcodec copy -acodec copy {vfixedmp4}";
		if (File.Exists("ffmpeg.exe") & File.Exists(vfixed))
		{
			writestatus("*****************************");
			writestatus("Phase 4 started!");
			if (File.Exists(vfixedmp4))
			{
				File.Delete(vfixedmp4);
			}
			string logresponse = startlog();
			dofix(text, text2, delegate(string reply)
			{
				Invoke((MethodInvoker)delegate
				{
					TextBox textBox = textBox3;
					textBox.Text = textBox.Text + reply + Environment.NewLine;
					logresponse = logresponse + reply + Environment.NewLine;
					if (reply.Contains("video:"))
					{
						Invoke((MethodInvoker)delegate
						{
							label1.Text = "Phase 4 100% Complete!";
							progressBar1.Value = 100;
							progressBar1.Style = ProgressBarStyle.Blocks;
						});
						writestatus("Successfully Created");
						writestatus(vfixedmp4);
						writestatus("Double Click Here to test");
						if (File.Exists(vfixedmp4))
						{
							vtest = vfixedmp4;
						}
						path = "Log_Ph4.txt";
						createlog(path, logresponse);
						FASE = 99;
						successreset(4);
						ERR = false;
					}
				});
			});
		}
		else
		{
			writestatus("Something went wrong last phase not done!!! Please check the logs and try again");
		}
	}

	private void task3()
	{
		disablebuttons();
		if (progressBar1.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				progressBar1.Value = 0;
				progressBar1.Style = ProgressBarStyle.Marquee;
			});
		}
		else
		{
			progressBar1.Value = 0;
			progressBar1.Style = ProgressBarStyle.Marquee;
		}
		ExtractResource("ffmpeg", "ffmpeg.exe");
		string text = "";
		infobad(vh264);
		text = string.Format("-r {0} -i \"{1}\" -vcodec copy -acodec copy {2}", FR_vh264, vh264.Replace("\\", "\\\\"), vfixed);
		if (File.Exists("ffmpeg.exe") & File.Exists(vh264))
		{
			writestatus("*****************************");
			writestatus("Phase 3 started!");
			if (File.Exists(vfixed))
			{
				File.Delete(vfixed);
			}
			string logresponse = startlog();
			dofix("ffmpeg", text, delegate(string reply)
			{
				Invoke((MethodInvoker)delegate
				{
					TextBox textBox = textBox3;
					textBox.Text = textBox.Text + reply + Environment.NewLine;
					logresponse = logresponse + reply + Environment.NewLine;
					if (reply.Contains("video:"))
					{
						Invoke((MethodInvoker)delegate
						{
							label1.Text = "Phase 3 100% Complete!";
							progressBar1.Style = ProgressBarStyle.Blocks;
							progressBar1.Value = 100;
						});
						writestatus("Successfully Created");
						writestatus(vfixed);
						writestatus("Double Click Here to test");
						if (checkBox1.Checked)
						{
							FASE = 4;
						}
						else
						{
							FASE = 99;
						}
						if (File.Exists(vfixed))
						{
							vtest = vfixed;
						}
						path = "Log_Ph3.txt";
						createlog(path, logresponse);
						successreset(3);
						ERR = false;
					}
				});
			});
		}
		else
		{
			textBox3.Text = $"Something went wrong!!!{Environment.NewLine}Please check the logs and try again";
		}
	}

	private void task2()
	{
		disablebuttons();
		if (checkfile(vbad))
		{
			writestatus("Running Phase 2...");
			long length = new FileInfo(vbad).Length;
			if (File.Exists(vh264))
			{
				File.Delete(vh264);
			}
			ExtractResource("fixer.exe", "fixer.exe");
			string url = $"\"{vbad}\" {vh264}";
			Regex myRegex = new Regex("0[xX][0-9a-fA-F]+", RegexOptions.Singleline);
			Invoke((MethodInvoker)delegate
			{
				progressBar1.Style = ProgressBarStyle.Blocks;
			});
			logresponse = startlog();
			int v = 0;
			int c = 0;
			int t = trackBar1.Value;
			dofix("fixer.exe", url, delegate(string reply)
			{
				logresponse = logresponse + reply + Environment.NewLine;
				logresponse = logresponse + reply + Environment.NewLine;
				Match match = myRegex.Match(reply);
				if (match.Success)
				{
					string hexValue = match.Value.ToString();
					v = hextopercent(hexValue, length);
					if (v > c + t)
					{
						Invoke((MethodInvoker)delegate
						{
							label1.Text = v + "% Process Completion";
							progressBar1.Value = v;
							t = trackBar1.Value;
						});
						writestatus(reply);
						c = v;
					}
				}
				if (reply.Contains(".h264' created, size"))
				{
					Invoke((MethodInvoker)delegate
					{
						label1.Text = "Phase 2 100% Complete!";
						progressBar1.Value = 100;
						if (File.Exists(vh264))
						{
							textBox2.Text = vh264;
							vtest = vh264;
							checkbuttons();
							if (checkBox2.Checked)
							{
								FASE = 3;
							}
							else
							{
								FASE = 2;
							}
						}
					});
					writestatus("Phase 2 completed!");
					writestatus($"File {vh264} has been created!");
					writestatus($"If you want to proceed click Transcode Video to convert {vh264} to {vfixed}!");
					path = "Log_Ph2.txt";
					createlog(path, logresponse);
					successreset(2);
					ERR = false;
				}
			});
		}
		else
		{
			MessageBox.Show("Choose a video to Fix");
		}
	}

	private void task1()
	{
		disablebuttons();
		logresponse = startlog();
		string url = $"\"{vgood}\" --avcc";
		writestatus("Processing File...");
		writestatus("Please Wait, if the file is too big this may take some time. please be patient...");
		ExtractResource("recover", "fixer.exe");
		if (checkfile("fixer.exe"))
		{
			Invoke((MethodInvoker)delegate
			{
				progressBar1.Style = ProgressBarStyle.Marquee;
			});
			if (File.Exists("avcc.hdr"))
			{
				File.Delete("avcc.hdr");
			}
			dofix("fixer.exe", url, delegate(string reply)
			{
				writestatus(reply);
				logresponse = logresponse + reply + Environment.NewLine;
				if (reply.Contains("'avcc.hdr' created"))
				{
					path = "Log_Ph1.txt";
					createlog(path, logresponse);
					ERR = false;
					successreset(1);
					FASE = 2;
				}
			});
		}
	}

	private string getdjifix_type(int task_optionindex)
	{
		switch (task_optionindex)
		{
		default:
			djifix_type = "1";
			break;
		case 0:
			if (!File.Exists(vgood))
			{
				MessageBox.Show("You need to Choose your file or format");
				textBox1c.Focus();
				return "";
			}
			break;
		case 1:
			djifix_type = "0";
			break;
		case 2:
			djifix_type = "1";
			break;
		case 3:
			djifix_type = "2";
			break;
		case 4:
			djifix_type = "3";
			break;
		case 5:
			djifix_type = "4";
			break;
		case 6:
			djifix_type = "5";
			break;
		case 7:
			djifix_type = "6";
			break;
		case 8:
			djifix_type = "7";
			break;
		case 9:
			djifix_type = "8";
			break;
		case 10:
			djifix_type = "9";
			break;
		case 11:
			djifix_type = "A";
			break;
		case 12:
			djifix_type = "B";
			break;
		case 13:
			djifix_type = "C";
			break;
		case 14:
			djifix_type = "D";
			break;
		case 15:
			djifix_type = "E";
			break;
		}
		return djifix_type;
	}

	private void infobad(string video)
	{
		MediaFile mediaFile = new MediaFile(video);
		double num = 0.0;
		string text = "";
		string text2 = "";
		string ret;
		try
		{
			num = mediaFile.Video[0].frameRate;
			text = mediaFile.Video[0].height.ToString();
			text2 = mediaFile.Video[0].width.ToString();
			ret = string.Format("{0}  h:{1} x w:{2}", num.ToString("0.00"), text, text2);
			FR_vh264 = num.ToString("0.00");
		}
		catch
		{
			ret = "";
		}
		Invoke((MethodInvoker)delegate
		{
			lbvbad.Text = ret;
			if (lbvbad.Text == "")
			{
				lbckvbad.Visible = true;
				lbckvbad.Text = "✗";
				lbckvbad.ForeColor = Color.Red;
			}
			else
			{
				lbckvbad.Visible = true;
				lbckvbad.Text = "✓";
				lbckvbad.ForeColor = Color.Green;
			}
		});
	}

	private void infogood(string video)
	{
		MediaFile mediaFile = new MediaFile(video);
		double num = 0.0;
		string text = "";
		string text2 = "";
		string ret;
		try
		{
			num = mediaFile.Video[0].frameRate;
			text = mediaFile.Video[0].height.ToString();
			text2 = mediaFile.Video[0].width.ToString();
			ret = string.Format("{0}  h:{1} x w:{2}", num.ToString("0.00"), text, text2);
			FR_vh264 = num.ToString("0.00");
		}
		catch
		{
			ret = "";
		}
		Invoke((MethodInvoker)delegate
		{
			lbvgood.Text = ret;
			if (lbvgood.Text == "")
			{
				lbckvgood.Visible = true;
				lbckvgood.Text = "✗";
				lbckvgood.ForeColor = Color.Red;
			}
			else
			{
				lbckvgood.Visible = true;
				lbckvgood.Text = "✓";
				lbckvgood.ForeColor = Color.Green;
			}
		});
	}

	private void ExtractResource(string resName, string fName)
	{
		bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
		fName = Path.Combine(Application.StartupPath.ToString(), fName);
		if (is64BitOperatingSystem & (resName == "recover"))
		{
			resName += "64";
		}
		if (!File.Exists(fName))
		{
			object @object = Resources.ResourceManager.GetObject(resName);
			byte[] array = (byte[])@object;
			using (FileStream fileStream = new FileStream(fName, FileMode.Create, FileAccess.Write))
			{
				byte[] array2 = array;
				fileStream.Write(array2, 0, array2.Length);
				fileStream.Close();
				fileStream.Dispose();
			}
		}
	}

	private bool checkforerror(string reply)
	{
		bool result = false;
		string[] array = new string[2]
		{
			"Fixed.h264: Unknown format",
			"We cannot repair this file"
		};
		string[] array2 = array;
		foreach (string value in array2)
		{
			if (result = reply.Contains(value))
			{
				break;
			}
		}
		return result;
	}

	private int hextopercent(string hexValue, double length)
	{
		hexValue = hexValue.Replace("0x", "");
		int value = int.Parse(hexValue, NumberStyles.HexNumber);
		double value2 = Convert.ToDouble(value) * 100.0 / Convert.ToDouble(length);
		return (int)Math.Round(value2, 0);
	}

	private void writestatus(string message)
	{
		SetText(textBox3, message + Environment.NewLine);
	}

	private void SetText(TextBox txt, string text)
	{
		if (txt.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				txt.Text += text;
			});
		}
		else
		{
			txt.Text += text;
		}
	}

	private bool checkfile(string fil)
	{
		bool result = true;
		if (!File.Exists(fil))
		{
			result = false;
			MessageBox.Show($"{fil} Missing!!!");
		}
		return result;
	}

	private void createlog(string path, string logresponse)
	{
		if (!File.Exists(path))
		{
			File.WriteAllText(path, logresponse);
		}
		else
		{
			File.AppendAllText(path, logresponse);
		}
	}

	private string startlog()
	{
		Invoke((MethodInvoker)delegate
		{
			textBox3.Text = "";
		});
		string str = "**************************************" + Environment.NewLine;
		str += $"New Log started: {DateTime.Now}{Environment.NewLine}";
		return str + "**************************************" + Environment.NewLine;
	}

	private string getmediainfo(string exe, string vgood)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = exe,
			Arguments = vgood,
			CreateNoWindow = true,
			StandardOutputEncoding = Encoding.UTF8,
			UseShellExecute = false,
			RedirectStandardOutput = true
		};
		using (Process process = Process.Start(startInfo))
		{
			using (StreamReader streamReader = process.StandardOutput)
			{
				return streamReader.ReadToEnd();
			}
		}
	}

	private void pictureBox1_Click(object sender, EventArgs e)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.FileName = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VLQ4X37KECHA8";
		process.Start();
	}

	private void trackBar1_Scroll(object sender, EventArgs e)
	{
		label8.Text = trackBar1.Value.ToString();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FIX_DJI.Form1));
		button1 = new System.Windows.Forms.Button();
		textBox1 = new System.Windows.Forms.TextBox();
		textBox2 = new System.Windows.Forms.TextBox();
		button2 = new System.Windows.Forms.Button();
		button3 = new System.Windows.Forms.Button();
		textBox3 = new System.Windows.Forms.TextBox();
		progressBar1 = new System.Windows.Forms.ProgressBar();
		label1 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		tbfase1 = new System.Windows.Forms.TextBox();
		tbfase2 = new System.Windows.Forms.TextBox();
		tbfase3 = new System.Windows.Forms.TextBox();
		label3 = new System.Windows.Forms.Label();
		checkBox1 = new System.Windows.Forms.CheckBox();
		label5 = new System.Windows.Forms.Label();
		textBox1c = new System.Windows.Forms.ComboBox();
		button5 = new System.Windows.Forms.Button();
		label6 = new System.Windows.Forms.Label();
		lbvbad = new System.Windows.Forms.Label();
		lbvgood = new System.Windows.Forms.Label();
		pictureBox1 = new System.Windows.Forms.PictureBox();
		lbckvgood = new System.Windows.Forms.Label();
		lbckvbad = new System.Windows.Forms.Label();
		checkBox2 = new System.Windows.Forms.CheckBox();
		trackBar1 = new System.Windows.Forms.TrackBar();
		label9 = new System.Windows.Forms.Label();
		label7 = new System.Windows.Forms.Label();
		label8 = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
		SuspendLayout();
		button1.Enabled = false;
		button1.Location = new System.Drawing.Point(303, 9);
		button1.Name = "button1";
		button1.Size = new System.Drawing.Size(85, 53);
		button1.TabIndex = 5;
		button1.Text = "Browse for Good Video...";
		button1.UseVisualStyleBackColor = true;
		button1.Click += new System.EventHandler(button1_Click);
		textBox1.Location = new System.Drawing.Point(24, 547);
		textBox1.Name = "textBox1";
		textBox1.Size = new System.Drawing.Size(267, 20);
		textBox1.TabIndex = 1;
		textBox1.Visible = false;
		textBox2.Location = new System.Drawing.Point(44, 99);
		textBox2.Name = "textBox2";
		textBox2.ReadOnly = true;
		textBox2.Size = new System.Drawing.Size(250, 20);
		textBox2.TabIndex = 4;
		button2.Location = new System.Drawing.Point(304, 84);
		button2.Name = "button2";
		button2.Size = new System.Drawing.Size(84, 35);
		button2.TabIndex = 6;
		button2.Text = "Video to Fix or h264";
		button2.UseVisualStyleBackColor = true;
		button2.Click += new System.EventHandler(button2_Click);
		button3.Enabled = false;
		button3.Location = new System.Drawing.Point(394, 9);
		button3.Name = "button3";
		button3.Size = new System.Drawing.Size(83, 53);
		button3.TabIndex = 7;
		button3.Text = "Recover Video";
		button3.UseVisualStyleBackColor = true;
		button3.Click += new System.EventHandler(button3_Click);
		textBox3.Font = new System.Drawing.Font("Courier New", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		textBox3.Location = new System.Drawing.Point(24, 203);
		textBox3.Multiline = true;
		textBox3.Name = "textBox3";
		textBox3.ReadOnly = true;
		textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		textBox3.Size = new System.Drawing.Size(453, 194);
		textBox3.TabIndex = 11;
		textBox3.Text = resources.GetString("textBox3.Text");
		textBox3.TextChanged += new System.EventHandler(textBox3_TextChanged_1);
		textBox3.DoubleClick += new System.EventHandler(textBox3_DoubleClick);
		progressBar1.Location = new System.Drawing.Point(24, 417);
		progressBar1.Name = "progressBar1";
		progressBar1.Size = new System.Drawing.Size(443, 23);
		progressBar1.TabIndex = 6;
		label1.AutoSize = true;
		label1.Location = new System.Drawing.Point(24, 401);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(0, 13);
		label1.TabIndex = 7;
		label2.AutoSize = true;
		label2.Location = new System.Drawing.Point(24, 177);
		label2.Name = "label2";
		label2.Size = new System.Drawing.Size(83, 13);
		label2.TabIndex = 8;
		label2.Text = "Console Results";
		label4.Location = new System.Drawing.Point(29, 511);
		label4.Name = "label4";
		label4.Size = new System.Drawing.Size(326, 21);
		label4.TabIndex = 11;
		label4.Text = "If you like this program please donate $1 for future developements";
		tbfase1.BackColor = System.Drawing.SystemColors.Control;
		tbfase1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		tbfase1.ForeColor = System.Drawing.SystemColors.Control;
		tbfase1.Location = new System.Drawing.Point(24, 155);
		tbfase1.Multiline = true;
		tbfase1.Name = "tbfase1";
		tbfase1.Size = new System.Drawing.Size(147, 16);
		tbfase1.TabIndex = 40;
		tbfase1.Text = "Phase 1 Running...";
		tbfase1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		tbfase2.BackColor = System.Drawing.SystemColors.Control;
		tbfase2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		tbfase2.ForeColor = System.Drawing.SystemColors.Control;
		tbfase2.Location = new System.Drawing.Point(172, 155);
		tbfase2.Multiline = true;
		tbfase2.Name = "tbfase2";
		tbfase2.Size = new System.Drawing.Size(147, 16);
		tbfase2.TabIndex = 41;
		tbfase2.Text = "Phase 2 Running...";
		tbfase2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		tbfase3.BackColor = System.Drawing.SystemColors.Control;
		tbfase3.BorderStyle = System.Windows.Forms.BorderStyle.None;
		tbfase3.ForeColor = System.Drawing.SystemColors.Control;
		tbfase3.Location = new System.Drawing.Point(320, 155);
		tbfase3.Multiline = true;
		tbfase3.Name = "tbfase3";
		tbfase3.Size = new System.Drawing.Size(147, 16);
		tbfase3.TabIndex = 42;
		tbfase3.Text = "Phase 3 Running...";
		tbfase3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		label3.AutoSize = true;
		label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		label3.Location = new System.Drawing.Point(24, 83);
		label3.Name = "label3";
		label3.Size = new System.Drawing.Size(260, 13);
		label3.TabIndex = 3;
		label3.Text = "Choose a Video File to Fix or a Video H264 to convert";
		checkBox1.AutoSize = true;
		checkBox1.Location = new System.Drawing.Point(396, 132);
		checkBox1.Name = "checkBox1";
		checkBox1.Size = new System.Drawing.Size(81, 17);
		checkBox1.TabIndex = 10;
		checkBox1.Text = "Output mp4";
		checkBox1.UseVisualStyleBackColor = true;
		label5.AutoSize = true;
		label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		label5.Location = new System.Drawing.Point(24, 9);
		label5.Name = "label5";
		label5.Size = new System.Drawing.Size(270, 13);
		label5.TabIndex = 0;
		label5.Text = "Browse for Good Video File with the same format or from";
		textBox1c.DropDownHeight = 108;
		textBox1c.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		textBox1c.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		textBox1c.FormattingEnabled = true;
		textBox1c.IntegralHeight = false;
		textBox1c.Items.AddRange(new object[16]
		{
			"Browse for a video File or choose your settings --->",
			"< 2160p (4k), 30 fps >",
			"< 2160p (4k), 25 fps >",
			"< 2160p (4k), 24 fps >",
			"< 1520p, 30 fps >",
			"< 1520p, 25 fps >",
			"< 1080p, 60 fps >",
			"< 1080i, 60 fps >",
			"< 1080p, 50 fps >",
			"< 1080p, 30 fps >",
			"< 1080p, 25 fps >",
			"< 1080p, 24 fps >",
			"< 720p, 60 fps >",
			"< 720p, 30 fps >",
			"< 720p, 25 fps >",
			"< 480p, 30 fps >"
		});
		textBox1c.Location = new System.Drawing.Point(44, 41);
		textBox1c.Name = "textBox1c";
		textBox1c.Size = new System.Drawing.Size(247, 21);
		textBox1c.TabIndex = 2;
		textBox1c.TextChanged += new System.EventHandler(textBox1c_TextChanged);
		button5.Enabled = false;
		button5.Location = new System.Drawing.Point(395, 84);
		button5.Name = "button5";
		button5.Size = new System.Drawing.Size(82, 35);
		button5.TabIndex = 8;
		button5.Text = "Transcode Video";
		button5.UseVisualStyleBackColor = true;
		button5.Click += new System.EventHandler(button5_Click);
		label6.AutoSize = true;
		label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		label6.Location = new System.Drawing.Point(24, 22);
		label6.Name = "label6";
		label6.Size = new System.Drawing.Size(261, 13);
		label6.TabIndex = 1;
		label6.Text = "the list, choose the format of the video you want to fix:";
		lbvbad.AutoSize = true;
		lbvbad.ForeColor = System.Drawing.SystemColors.HotTrack;
		lbvbad.Location = new System.Drawing.Point(48, 124);
		lbvbad.Name = "lbvbad";
		lbvbad.Size = new System.Drawing.Size(0, 13);
		lbvbad.TabIndex = 23;
		lbvgood.AutoSize = true;
		lbvgood.ForeColor = System.Drawing.SystemColors.HotTrack;
		lbvgood.Location = new System.Drawing.Point(48, 65);
		lbvgood.Name = "lbvgood";
		lbvgood.Size = new System.Drawing.Size(0, 13);
		lbvgood.TabIndex = 25;
		pictureBox1.Image = FIX_DJI.Properties.Resources.paypal;
		pictureBox1.Location = new System.Drawing.Point(374, 501);
		pictureBox1.Name = "pictureBox1";
		pictureBox1.Size = new System.Drawing.Size(103, 30);
		pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		pictureBox1.TabIndex = 9;
		pictureBox1.TabStop = false;
		pictureBox1.Click += new System.EventHandler(pictureBox1_Click);
		lbckvgood.AutoSize = true;
		lbckvgood.BackColor = System.Drawing.Color.Transparent;
		lbckvgood.Font = new System.Drawing.Font("Mistral", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		lbckvgood.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		lbckvgood.Location = new System.Drawing.Point(24, 44);
		lbckvgood.Name = "lbckvgood";
		lbckvgood.Size = new System.Drawing.Size(14, 16);
		lbckvgood.TabIndex = 27;
		lbckvgood.Text = "X";
		lbckvgood.Visible = false;
		lbckvbad.AutoSize = true;
		lbckvbad.BackColor = System.Drawing.Color.Transparent;
		lbckvbad.Font = new System.Drawing.Font("Mistral", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		lbckvbad.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		lbckvbad.Location = new System.Drawing.Point(24, 102);
		lbckvbad.Name = "lbckvbad";
		lbckvbad.Size = new System.Drawing.Size(14, 16);
		lbckvbad.TabIndex = 28;
		lbckvbad.Text = "X";
		lbckvbad.Visible = false;
		checkBox2.AutoSize = true;
		checkBox2.Checked = true;
		checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
		checkBox2.Location = new System.Drawing.Point(306, 132);
		checkBox2.Name = "checkBox2";
		checkBox2.Size = new System.Drawing.Size(86, 17);
		checkBox2.TabIndex = 9;
		checkBox2.Text = "h264 to Mov";
		checkBox2.UseVisualStyleBackColor = true;
		trackBar1.Location = new System.Drawing.Point(24, 465);
		trackBar1.Maximum = 30;
		trackBar1.Minimum = 2;
		trackBar1.Name = "trackBar1";
		trackBar1.Size = new System.Drawing.Size(453, 45);
		trackBar1.SmallChange = 2;
		trackBar1.TabIndex = 12;
		trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
		trackBar1.Value = 10;
		trackBar1.Scroll += new System.EventHandler(trackBar1_Scroll);
		label9.AutoSize = true;
		label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		label9.Location = new System.Drawing.Point(29, 447);
		label9.Name = "label9";
		label9.Size = new System.Drawing.Size(318, 13);
		label9.TabIndex = 33;
		label9.Text = "Slide to increase responses (slower) or decrease responses (faster)";
		label7.AutoSize = true;
		label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		label7.Location = new System.Drawing.Point(404, 446);
		label7.Name = "label7";
		label7.Size = new System.Drawing.Size(37, 13);
		label7.TabIndex = 43;
		label7.Text = "Value:";
		label8.AutoSize = true;
		label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		label8.Location = new System.Drawing.Point(437, 443);
		label8.Name = "label8";
		label8.Size = new System.Drawing.Size(27, 20);
		label8.TabIndex = 44;
		label8.Text = "10";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(499, 543);
		base.Controls.Add(label8);
		base.Controls.Add(label7);
		base.Controls.Add(label9);
		base.Controls.Add(pictureBox1);
		base.Controls.Add(label4);
		base.Controls.Add(trackBar1);
		base.Controls.Add(checkBox2);
		base.Controls.Add(lbckvbad);
		base.Controls.Add(lbvgood);
		base.Controls.Add(lbvbad);
		base.Controls.Add(label6);
		base.Controls.Add(button5);
		base.Controls.Add(textBox1c);
		base.Controls.Add(label5);
		base.Controls.Add(checkBox1);
		base.Controls.Add(label3);
		base.Controls.Add(tbfase3);
		base.Controls.Add(tbfase2);
		base.Controls.Add(tbfase1);
		base.Controls.Add(label2);
		base.Controls.Add(label1);
		base.Controls.Add(progressBar1);
		base.Controls.Add(textBox3);
		base.Controls.Add(button3);
		base.Controls.Add(button2);
		base.Controls.Add(textBox2);
		base.Controls.Add(textBox1);
		base.Controls.Add(button1);
		base.Controls.Add(lbckvgood);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "Form1";
		Text = "DJI Video Fixer";
		base.Load += new System.EventHandler(Form1_Load);
		((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}
}
