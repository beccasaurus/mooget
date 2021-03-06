using System;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class SaySpec : MooGetSpec {

		[Test]
		public void can_moo_1_line_of_text() {
			var text = @"
 _____________
< hello world >
 -------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||
";
		moo("cow hello world").ShouldEqual(text.TrimStart('\n').TrimEnd('\n'));
		}
		
		[Test]
		public void can_moo_2_lines_of_text() {
			var text = @"
 ____________________________________
/ hello world how goes it? i am long \
\ enough to have 2 lines             /
 ------------------------------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||
";
			moo("cow hello world how goes it? i am long enough to have 2 lines").ShouldEqual(text.TrimStart('\n').TrimEnd('\n'));
		}
		
		[Test]
		public void can_moo_many_lines_of_text() {
			var text = @"
 _______________________________________
/ hello world how goes it? i was long   \
| enough to have 2 lines but now I am   |
\ much longer and I should have 3 lines /
 ---------------------------------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||
";
			moo("cow hello world how goes it? i was long enough to have 2 lines but now I am much longer and I should have 3 lines").ShouldEqual(text.TrimStart('\n').TrimEnd('\n'));
		}
	}
}
