import globalColors from "../utils/globalColors";

const {
  black,
  white,
  whiteSolitude,
  grayLight,
  grayLightMid,
  grayMid,
  graySilver,
  gray,
  grayMain,
  shuttleGrey,

  blueMain,
  blueHover,
  blueActive,
  blueDisabled,
  blueCharcoal,

  orangeMain,
  orangeHover,
  orangePressed,
  orangeDisabled,

  link,
  errorColor,
  warningColor,
  red
} = globalColors;

const Dark = {
  color: whiteSolitude,
  backgroundColor: blueCharcoal,
  fontFamily: "Open Sans, sans-serif, Arial",

  text: {
    color: white,
    disableColor: grayLightMid,
    fontWeight: "normal",
    fontWeightBold: "bold"
  },

  heading: {
    fontSize: {
      xlarge: "27px",
      large: "23px",
      medium: "21px",
      small: "19px",
      xsmall: "15px"
    },
    fontWeight: 600
  },

  button: {
    fontWeight: 600,
    margin: 0,
    display: "inline-block",
    textAlign: "center",
    textDecoration: "none",
    verticalAlign: "middle",
    borderRadius: "3px",
    stroke: "none",
    overflow: "hidden",
    textOverflow: "ellipsis",
    whiteSpace: "nowrap",
    outline: "none",
    height: {
      base: "24px",
      medium: "32px",
      big: "36px"
    },
    lineHeight: {
      base: "normal",
      medium: "normal",
      big: "normal"
    },
    fontSize: {
      base: "12px",
      medium: "13px",
      big: "14px"
    },
    padding: {
      base: "0 14px",
      medium: "0 18px",
      big: "0 20px"
    },
    minWidth: {
      base: "65px",
      medium: "80px",
      big: "85px"
    },
    color: {
      base: black,
      primary: white,
      disabled: grayMid
    },
    backgroundColor: {
      base: white,
      baseHover: white,
      baseActive: grayLightMid,
      baseDisabled: grayLight,
      primary: blueMain,
      primaryHover: blueHover,
      primaryActive: blueActive,
      primaryDisabled: blueDisabled
    },
    border: {
      base: `1px solid ${grayMid}`,
      baseHover: `1px solid ${blueMain}`,
      baseActive: `1px solid ${blueMain}`,
      baseDisabled: `1px solid ${grayLightMid}`,
      primary: `1px solid ${blueMain}`,
      primaryHover: `1px solid ${blueHover}`,
      primaryActive: `1px solid ${blueActive}`,
      primaryDisabled: `1px solid ${blueDisabled}`
    }
  },

  checkbox: {
    fillColor: white,
    borderColor: grayMid,
    arrowColor: black,
    indeterminateColor: black,

    disableArrowColor: grayMid,
    disableBorderColor: grayLightMid,
    disableFillColor: grayLight,
    disableIndeterminateColor: gray,

    hoverBorderColor: gray,
    hoverIndeterminateColor: black
  },

  switchButton: {
    fillColor: white,
    checkedFillColor: gray,

    fillColorDisabled: grayLight,
    disabledFillColor: grayLightMid,
    disabledFillColorInner: grayMid,
    
    hoverBorderColor: gray
  },

  toggleButton: {
    fillColor: blueMain,
    fillColorOff: gray,

    disableFillColor: grayLightMid,
    disableFillColorOff: grayLightMid
  },

  radioButton: {
    color: white,
    disableColor: grayLightMid,
    width: "16px",
    height: "16px",

    fillColor: black,
    borderColor: grayMid,

    disableFillColor: grayMid,
    disableBorderColor: grayLightMid,

    hoverBorderColor: gray
  },

  scrollbar: {
    backgroundColorVertical: "rgba(255, 255, 255, 0.5)",
    backgroundColorHorizontal: "rgba(255, 255, 255, 0.5)"
  },

  input: {
    color: white,
    disableColor: grayMid,

    backgroundColor: blueCharcoal,
    disableBackgroundColor: shuttleGrey,

    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px",
      large: "550px"
    },

    borderRadius: "3px",
    boxShadow: "none",
    boxSizing: "border-box",
    border: "solid 1px",

    borderColor: grayMid,
    errorBorderColor: red,
    warningBorderColor: warningColor,
    disabledBorderColor: grayLightMid,

    hoverBorderColor: grayMid,
    hoverErrorBorderColor: red,
    hoverWarningBorderColor: warningColor,
    hoverDisabledBorderColor: gray,

    focusBorderColor: blueMain,
    focusErrorBorderColor: red,
    focusWarningBorderColor: warningColor,
    focusDisabledBorderColor: grayLightMid
  },

  textInput: {
    fontWeight: "normal",
    placeholderColor: grayMid,
    disablePlaceholderColor: grayMid,

    transition: "all 0.2s ease 0s",
    appearance: "none",
    display: "flex",
    flex: "1 1 0%",
    outline: "none",
    overflow: "hidden",

    lineHeight: {
      base: "20px",
      middle: "20px",
      big: "20px",
      huge: "21px"
    },

    fontSize: {
      base: "13px",
      middle: "14px",
      big: "16px",
      huge: "18px"
    },

    padding: {
      base: "5px 7px",
      middle: "8px 12px",
      big: "8px 16px",
      huge: "8px 20px"
    }
  },

  textArea: {
    width: "100%",
    height: "90%",
    border: "none",
    outline: "none",
    resize: "none",
    overflow: "hidden",
    padding: "5px 8px 2px 8px",
    fontSize: "13px",
    lineHeight: "1.5",

    disabledColor: grayLight,

    focusBorderColor: blueMain,
    focusOutline: "none",

    scrollWidth: "100% !important",
    scrollHeight: "91px !important"
  },

  link: {
    color: white,
    lineHeight: "calc(100% + 6px)",
    opacity: "0.5",
    textDecoration: "none",
    cursor: "pointer",

    hover: {
      textDecoration: "underline dashed",
      page: { textDecoration: "underline" }
    }
  },

  tooltip: {
    borderRadius: "6px",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    opacity: "1",
    padding: "16px",
    pointerEvents: "auto",
    maxWidth: "340px",
    backgroundColor: grayMid,
    before: {
      border: "none"
    },
    after: {
      border: "none"
    }
  },

  avatar: {
    initialsContainer: {
      color: white,
      left: "50%",
      top: "50%",
      transform: "translate(-50%, -50%)",
      fontWeight: "600"
    },

    imageContainer: {
      backgroundImage: blueMain,
      background: grayLightMid,
      borderRadius: "50%",
      height: "100%",

      svg: {
        display: "block",
        width: "50%",
        height: "100%",
        margin: "auto"
      }
    },

    image: {
      width: "100%",
      height: "100%",
      borderRadius: "50%"
    },

    width: {
      small: "32px",
      medium: "48px",
      big: "82px",
      max: "160px"
    },
    height: {
      small: "32px",
      medium: "48px",
      big: "82px",
      max: "160px"
    },
    fontSize: {
      small: "12px",
      medium: "20px",
      big: "34px",
      max: "72px"
    }
  },

  backdrop: { backgroundColor: "rgba(255, 255, 255, 0.3)" },

  loader: {
    color: white,
    size: "40px",
    ovalFill: blueCharcoal,
    strokeWidth: 2
  },

  dropDownItem: {
    width: "100%",
    maxWidth: "240px",
    border: "none",
    cursor: "pointer",
    padding: "0px 16px",
    lineHeight: "32px",
    textAlign: "left",
    background: "none",
    textDecoration: "none",
    fontStyle: "normal",
    fontWeight: "600",
    fontSize: "13px",

    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis",

    outline: "none",
    color: black,
    textTransform: "none",

    hoverBackgroundColor: grayLight,
    noHoverBackgroundColor: white,

    header: {
      color: gray,
      hoverCursor: "default",
      hoverBackgroundColor: "white",
      textTransform: "uppercase"
    },

    disabled: {
      color: gray,
      hoverCursor: "default",
      hoverBackgroundColor: "white"
    },

    separator: {
      padding: "0px 16px",
      border: `0.5px solid ${grayLightMid}`,
      cursor: "default",
      margin: "6px 16px 6px",
      lineHeight: "1px",
      height: "1px",
      width: "calc(100% - 32px)"
    },

    tablet: { lineHeight: "36px" }
  },

  dropDown: {
    zIndex: "150",
    background: white,
    borderRadius: "6px",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    padding: "6px 0px"
  },

  comboBox: {
    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px"
    },

    arrow: {
      width: "8px",
      flex: "0 0 8px",
      marginTopWithBorder: "5px",
      marginTop: "12px",
      marginRight: "8px",
      marginLeft: "auto",
      fillColor: gray
    },

    button: {
      height: "18px",
      heightWithBorder: "32px",
      paddingLeft: "8px",

      color: black,
      disabledColor: grayMid,
      background: white,
      backgroundWithBorder: "none",

      border: `1px solid ${grayMid}`,
      borderRadius: "3px",
      borderColor: white,

      disabledBorderColor: grayLightMid,
      disabledBackground: grayLight,

      hoverBorderColor: gray,
      hoverBorderColorOpen: blueMain,
      hoverDisabledBorderColor: grayLightMid
    },

    label: {
      marginRightWithBorder: "8px",
      marginRight: "4px",

      defaultDisabledColor: grayMid,
      defaultColor: gray,
      disabledColor: grayMid,
      color: white,

      maxWidth: "175px",

      lineHeightWithoutBorder: "15px",
      lineHeightTextDecoration: "underline dashed transparent"
    },

    childrenButton: {
      marginRight: "8px",
      defaultDisabledColor: grayMid,
      defaultColor: gray,
      disabledColor: grayMid,
      color: black
    }
  },
  toggleContent: {
    headingHeight: "24px",
    headingLineHeight: "18px",
    hoverBorderBottom: "1px dashed",
    contentPadding: "10px 0px 0px 0px",
    arrowMargin: "4px 8px 4px 0px",
    transform: "rotate(180deg)",
    iconColor: white
  },

  calendar: {
    baseWidth: "265px",
    bigWidth: "289px",

    hover: {
      backgroundColor: shuttleGrey,
      borderRadius: "16px",
      cursor: "pointer"
    },

    day: {
      width: "32px",
      height: "32px",
      baseSizeWidth: "270px",
      bigSizeWidth: "294px",
      baseMarginTop: "3px",
      bigMarginTop: "7.5px",
      lineHeight: "33px"
    },
    weekdays: {
      color: white,
      disabledColor: "#A3A9AE",
      baseWidth: "272px",
      bigWidth: "295px",
      marginBottom: "-5px"
    },
    month: {
      baseWidth: "267px",
      bigWidth: "295px",
      color: white,
      weekendColor: gray,
      disabledColor: shuttleGrey,
      neighboringHoverColor: white,
      neighboringColor: grayMain
    },
    selectedDay: {
      backgroundColor: orangeMain,
      borderRadius: "16px",
      cursor: "pointer",
      color: white
    },
    comboBox: {
      color: white,
      minWidth: "80px",
      height: "32px",
      marginLeft: "8px",
      padding: "0 0 24px 0"
    },
    comboBoxMonth: {
      baseWidth: "172px",
      bigWidth: "196px"
    }
  },

  datePicker: {
    width: "110px",
    dropDownPadding: "16px 16px 16px 17px",
    inputPadding: "5px 5px 5px 9px",
    inputBorder: blueMain,
    iconPadding: "8px 8px 7px 0px"
  },

  phoneInput: {
    width: "304px",
    height: "44px",
    itemTextColor: black ,
    itemBackgroundColor: white,
    itemHoverColor: grayLightMid,
    scrollBackground: "rgba(0, 0, 0, 0.1)",
    placeholderColor: gray
  },

  squareButton: {
    height: "32px",
    width: "32px",
    color: gray,
    backgroundColor: white,
    border: `1px solid ${grayMid}`,
    borderRadius: "3px",
    outline: "none",
    hover: {
      backgroundColor: white,
      border: `1px solid ${gray}`
    },
    click: {
      backgroundColor: grayLightMid,
      border: `1px solid ${gray}`
    },
    disable: {
      backgroundColor: grayLight,
      border: `1px solid ${grayLightMid}`
    },
    crossShape: {
      color: graySilver,
      disable: {
        color: gray
      }
    }
  },

  roundButton: {
    height: "40px",
    width: "40px",
    backgroundColor: grayLight,
    borderRadius: 
    {
      plus: "112px",
      minus: "81px"
    },
    borderStyle: "none",
    outline: "none",
    hover: {
      backgroundColor: grayLightMid,
    },
    click: {
      backgroundColor: grayMid,
    },
    disable: {
      backgroundColor: grayLight,
    },
    plus: {
      color: grayMid,
      disable: {
        color: black
      }
    }
  }
};

export default Dark;
