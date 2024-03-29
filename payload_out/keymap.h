/*
	Goose Net
	Goose net is an open source hardware attack botnet written in c# / processing
	and is used in conjunction with a bad usb at_tiny85 arduino usb keyboard emulator for use in a penetration
	testing environment.

	GNU General Public License(GPL) version 3

	Copyright(c) 2023 Anthony Logan Mitchell

	This program is free software : you can redistribute it and /or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.If not, see <https://www.gnu.org/licenses/>.
*/
#define	RESERVED_NO_EVENT	0x00
#define	KEY_ERRORROLLOVER	0x01
#define	KEY_POSTFAIL	0x02
#define	KEY_ERRORUNDEFINED	0x03
#define	KEY_A	0x04
#define	KEY_B	0x05
#define	KEY_C	0x06
#define	KEY_D	0x07
#define	KEY_E	0x08
#define	KEY_F	0x09
#define	KEY_G	0x0A
#define	KEY_H	0x0B
#define	KEY_I	0x0C
#define	KEY_J	0x0D
#define	KEY_K	0x0E
#define	KEY_L	0x0F
#define	KEY_M	0x10
#define	KEY_N	0x11
#define	KEY_O	0x12
#define	KEY_P	0x13
#define	KEY_Q	0x14
#define	KEY_R	0x15
#define	KEY_S	0x16
#define	KEY_T	0x17
#define	KEY_U	0x18
#define	KEY_V	0x19
#define	KEY_W	0x1A
#define	KEY_X	0x1B
#define	KEY_Y	0x1C
#define	KEY_Z	0x1D
#define	KEY_1	0x1E
#define	KEY_2	0x1F
#define	KEY_3	0x20
#define	KEY_4	0x21
#define	KEY_5	0x22
#define	KEY_6	0x23
#define	KEY_7	0x24
#define	KEY_8	0x25
#define	KEY_9	0x26
#define	KEY_0	0x27
#define	KEY_ENTER	0x28
#define	KEY_ESCAPE	0x29
#define	KEY_BACKSPACE	0x2A
#define	KEY_TAB	0x2B
#define	KEY_SPACEBAR	0x2C
#define	KEY_UNDERSCORE	0x2D
#define	KEY_EQUALS	0x2E
#define	KEY_SQUARELEFT	0x2F
#define	KEY_SQUARERIGHT	0x30
#define	KEY_BACKSLASH	0x31
#define	KEY_POUND	0x32
#define	KEY_SEMICOLON	0x33
#define	KEY_QUOTE	0x34
#define	KEY_GRAVE_AND_TILDE	0x35
#define	KEY_COMMA	0x36
#define	KEY_PERIOD	0x37
#define	KEY_FRONTSLASH	0x38
#define	KEY_CAPS_LOCK	0x39
#define	KEY_F1	0x3A
#define	KEY_F2	0x3B
#define	KEY_F3	0x3C
#define	KEY_F4	0x3D
#define	KEY_F5	0x3E
#define	KEY_F6	0x3F
#define	KEY_F7	0x40
#define	KEY_F8	0x41
#define	KEY_F9	0x42
#define	KEY_F10	0x43
#define	KEY_F11	0x44
#define	KEY_F12	0x45
#define	KEY_PRINTSCREEN	0x46
#define	KEY_SCROLL_LOCK	0x47
#define	KEY_PAUSE	0x48
#define	KEY_INSERT	0x49
#define	KEY_HOME	0x4A
#define	KEY_PAGEUP	0x4B
#define	KEY_DELETE	0x4C
#define	KEY_END	0x4D
#define	KEY_PAGEDOWN	0x4E
#define	KEY_RIGHTARROW	0x4F
#define	KEY_LEFTARROW	0x50
#define	KEY_DOWNARROW	0x51
#define	KEY_UPARROW	0x52
#define	KEYPAD_NUMLOCK	0x53
#define	KEYPAD_FORWARDSLASH	0x54
#define	KEYPAD_ASTERISK	0x55
#define	KEYPAD_MINUS	0x56
#define	KEYPAD_PLUS	0x57
#define	KEYPAD_ENTER	0x58
#define	KEYPAD_1_AND_END	0x59
#define	KEYPAD_2_AND_DOWN_ARROW	0x5A
#define	KEYPAD_3_AND_PAGEDN	0x5B
#define	KEYPAD_4_AND_LEFT_ARROW	0x5C
#define	KEYPAD_5	0x5D
#define	KEYPAD_6_AND_RIGHT_ARROW	0x5E
#define	KEYPAD_7_AND_HOME	0x5F
#define	KEYPAD_8_AND_UP_ARROW	0x60
#define	KEYPAD_9_AND_PAGEUP	0x61
#define	KEYPAD_0_AND_INSERT	0x62
#define	KEYPAD_._AND_DELETE	0x63
#define	KEY_NON-US_BACKSLASH	0x64
#define	KEY_APPLICATION	0x65
#define	KEY_POWER	0x66
#define	KEYPAD_EQUALS	0x67
#define	KEY_F13	0x68
#define	KEY_F14	0x69
#define	KEY_F15	0x6A
#define	KEY_F16	0x6B
#define	KEY_F17	0x6C
#define	KEY_F18	0x6D
#define	KEY_F19	0x6E
#define	KEY_F20	0x6F
#define	KEY_F21	0x70
#define	KEY_F22	0x71
#define	KEY_F23	0x72
#define	KEY_F24	0x73
#define	KEY_EXECUTE	0x74
#define	KEY_HELP	0x75
#define	KEY_MENU	0x76
#define	KEY_SELECT	0x77
#define	KEY_STOP	0x78
#define	KEY_AGAIN	0x79
#define	KEY_UNDO	0x7A
#define	KEY_CUT	0x7B
#define	KEY_COPY	0x7C
#define	KEY_PASTE	0x7D
#define	KEY_FIND	0x7E
#define	KEY_MUTE	0x7F
#define	KEY_VOLUME_UP	0x80
#define	KEY_VOLUME_DOWN	0x81
#define	KEY_LOCKING_CAPS_LOCK	0x82
#define	KEY_LOCKING_NUM_LOCK	0x83
#define	KEY_LOCKING_SCROLL_LOCK	0x84
#define	KEYPAD_COMMA	0x85
#define	KEYPAD_EQUAL_SIGN	0x86
#define	KEY_INTERNATIONAL1	0x87
#define	KEY_INTERNATIONAL2	0x88
#define	KEY_INTERNATIONAL3	0x89
#define	KEY_INTERNATIONAL4	0x8A
#define	KEY_INTERNATIONAL5	0x8B
#define	KEY_INTERNATIONAL6	0x8C
#define	KEY_INTERNATIONAL7	0x8D
#define	KEY_INTERNATIONAL8	0x8E
#define	KEY_INTERNATIONAL9	0x8F
#define	KEY_LANG1	0x90
#define	KEY_LANG2	0x91
#define	KEY_LANG3	0x92
#define	KEY_LANG4	0x93
#define	KEY_LANG5	0x94
#define	KEY_LANG6	0x95
#define	KEY_LANG7	0x96
#define	KEY_LANG8	0x97
#define	KEY_LANG9	0x98
#define	KEY_ALTERNATE_ERASE	0x99
#define	KEY_SYSREQ_ATTENTION	0x9A
#define	KEY_CANCEL	0x9B
#define	KEY_CLEAR	0x9C
#define	KEY_PRIOR	0x9D
#define	KEY_RETURN	0x9E
#define	KEY_SEPARATOR	0x9F
#define	KEY_OUT	0xA0
#define	KEY_OPER	0xA1
#define	KEY_CLEAR/AGAIN	0xA2
#define	KEY_CRSEL/PROPS	0xA3
#define	KEY_EXSEL	0xA4
#define	KEY_LEFTCONTROL	(1<<0)
#define	KEY_LEFTSHIFT	(1<<1)
#define	KEY_LEFTALT	(1<<2)
#define	KEY_LEFT_GUI	(1<<3)
#define	KEY_RIGHTCONTROL	(1<<4)
#define	KEY_RIGHTSHIFT	(1<<5)
#define	KEY_RIGHTALT	(1<<6)
#define	KEY_RIGHT_GUI	(1<<7)

#define MOD_CONTROL_LEFT    (1<<0)
#define MOD_SHIFT_LEFT      (1<<1)
#define MOD_ALT_LEFT        (1<<2)
#define MOD_GUI_LEFT        (1<<3)
#define MOD_CONTROL_RIGHT   (1<<4)
#define MOD_SHIFT_RIGHT     (1<<5)
#define MOD_ALT_RIGHT       (1<<6)
#define MOD_GUI_RIGHT       (1<<7)
