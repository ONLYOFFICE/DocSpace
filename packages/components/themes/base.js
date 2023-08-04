import globalColors from "../utils/globalColors";

import AvatarBaseReactSvgUrl from "PUBLIC_DIR/images/avatar.base.react.svg?url";

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

  blueDenim,
  blueDenimTransparent,
  blueMaya,
  blueSky,

  orangeMain,
  orangeHover,
  orangePressed,
  orangeDisabled,

  link,
  errorColor,
  warningColor,
  red,
  blueLightMid,
  grayMaxLight,
  cyanBlueDarkShade,
  lightCumulus,
  lightMediumGoldenrod,
  activeSuccess,
  activeError,
  activeInfo,
  activeWarning,
  hoverSuccess,
  hoverError,
  hoverInfo,
  hoverWarning,
  darkBlack,
  silver,
  lightHover,
  strongBlue,
  lightGrayishStrongBlue,
  darkRed,
} = globalColors;

const Base = {
  isBase: true,
  color: black,
  backgroundColor: white,
  fontFamily: "Open Sans, sans-serif, Arial",
  fontSize: "13px",

  text: {
    color: black,
    disableColor: gray,
    fontWeight: "normal",
    fontWeightBold: "bold",
  },

  heading: {
    fontSize: {
      xlarge: "27px",
      large: "23px",
      medium: "21px",
      small: "19px",
      xsmall: "15px",
    },

    fontWeight: 600,
    color: black,
  },

  button: {
    fontWeight: "600",
    margin: "0",
    display: "inline-block",
    textAlign: "center",
    textDecoration: "none",

    topVerticalAlign: "text-top",
    middleVerticalAlign: "middle",
    bottomVerticalAlign: "text-bottom",

    borderRadius: "3px",
    stroke: "none",
    overflow: "hidden",
    textOverflow: "ellipsis",
    whiteSpace: "nowrap",
    outline: "none",
    boxSizing: "border-box",

    paddingRight: "4px",

    height: {
      extraSmall: "24px",
      small: "32px",
      normal: "40px",
      medium: "44px",
    },

    lineHeight: {
      extraSmall: "15px",
      small: "20px",
      normal: "16px",
      medium: "22px",
    },

    fontSize: {
      extraSmall: "12px",
      small: "13px",
      normal: "14px",
      medium: "16px",
    },

    padding: {
      extraSmall: "0 11.5px",
      small: "0 28px",
      normal: "0 28px",
      medium: "0 32px",
    },

    color: {
      base: black,
      baseHover: black,
      baseActive: black,
      baseDisabled: grayMid,
      primary: white,
      primaryHover: white,
      primaryActive: white,
      primaryDisabled: white,
    },

    backgroundColor: {
      base: white,
      baseHover: white,
      baseActive: grayLightMid,
      baseDisabled: grayLight,

      primary: blueDenim,
      primaryHover: blueDenimTransparent,
      primaryActive: blueMaya,
      primaryDisabled: blueSky,
    },

    border: {
      base: `1px solid ${globalColors.grayMid}`,
      baseHover: `1px solid ${blueDenim}`,
      baseActive: `1px solid ${globalColors.grayMid}`,
      baseDisabled: `1px solid ${globalColors.grayLightMid}`,

      primary: `1px solid ${blueDenim}`,
      primaryHover: `1px solid ${blueDenimTransparent}`,
      primaryActive: `1px solid ${blueMaya}`,
      primaryDisabled: `1px solid ${blueSky}`,
    },

    loader: {
      base: blueDenim,
      primary: white,
    },
  },

  helpButton: {
    width: "100%",
    backgroundColor: white,
    maxWidth: "500px",
    margin: "0",
    lineHeight: "56px",
    fontWeight: "700",
    borderBottom: `1px solid ${globalColors.lightGrayishBlue}`,
    padding: "0 16px 16px",
    bodyPadding: "16px 0",
  },

  mainButtonMobile: {
    textColor: grayMain,

    buttonColor: orangeMain,
    iconFill: white,

    circleBackground: white,

    mobileProgressBarBackground: "rgb(48%, 58%, 69%, 0.4)",

    bar: {
      errorBackground: orangePressed,

      icon: "#A3A9AE",
    },

    buttonWrapper: {
      background: white,
      uploadingBackground: grayLightMid,
    },

    buttonOptions: {
      backgroundColor: blueLightMid,
      color: white,
    },

    dropDown: {
      position: "fixed",
      right: "32px",
      bottom: "32px",

      width: "400px",

      zIndex: "202",

      mobile: {
        right: "24px",
        bottom: "24px",

        marginLeft: "24px",

        width: "calc(100vw - 48px)",
      },
      separatorBackground: white,

      buttonColor: white,
      hoverButtonColor: "#3a6c9e",

      backgroundActionMobile: blueLightMid,
    },

    dropDownItem: {
      padding: "10px",
    },
  },

  mainButton: {
    backgroundColor: "#4781D1",
    disableBackgroundColor: "rgba(71, 129, 209, 0.6)",
    hoverBackgroundColor: "rgba(71, 129, 209, .85)",
    clickBackgroundColor: "#4074BC",

    padding: "5px 14px 5px 12px",
    borderRadius: "3px",
    lineHeight: "22px",
    fontSize: "16px",
    fontWeight: 700,
    textColor: white,
    textColorDisabled: white,

    cornerRoundsTopRight: "0",
    cornerRoundsBottomRight: "0",

    svg: {
      margin: "auto",
      height: "100%",
      fill: white,
    },

    dropDown: {
      top: "100%",
    },

    arrowDropdown: {
      borderLeft: "4px solid transparent",
      borderRight: "4px solid transparent",
      borderTop: "5px solid white",
      borderTopDisabled: `5px solid white`,
      right: "14px",
      top: "50%",
      width: "0",
      height: "0",
      marginTop: " -1px",
    },
  },

  socialButton: {
    fontWeight: "500",
    textDecoration: "none",
    padding: "0",
    borderRadius: "2px",
    height: "40px",
    heightSmall: "32px",
    textAlign: "left",
    stroke: " none",
    outline: "none",
    width: "100%",

    background: white,
    disableBackgroundColor: "#F8F9F9",
    connectBackground: "#3B72A7",
    hoverBackground: white,
    hoverConnectBackground: "#265A8F",
    activeBackground: "grayMaxLight",
    hoverBorder: "#1877f2",

    boxShadow:
      "0px 1px 1px rgba(0, 0, 0, 0.24),0px 0px 1px rgba(0, 0, 0, 0.12)",
    hoverBoxShadow: "none",

    color: "rgba(0, 0, 0, 0.54)",
    disableColor: "#333333",
    disabledSvgColor: "none",
    border: "none",
    text: {
      width: "100%",
      height: "16px",
      margin: "0 11px",
      fontWeight: "600",
      fontSize: "14px",
      lineHeight: "14px",
      letterSpacing: "0.21875px",
      overflow: "hidden",
      textOverflow: "ellipsis",
      whiteSpace: "nowrap",
      color: "#A3A9AE",
      hoverColor: black,
      connectColor: white,
    },

    svg: {
      margin: "11px 16px",
      width: "18px",
      height: "18px",
      minWidth: "18px",
      minHeight: "18px",
      fill: white,
    },
  },

  groupButton: {
    fontSize: "14px",
    lineHeight: "19px",
    color: black,
    disableColor: gray,
    float: "left",
    height: "19px",
    overflow: "hidden",
    padding: "0px",

    separator: {
      border: `1px solid ${globalColors.grayLightMid}`,
      width: "0px",
      height: "24px",
      margin: "16px 12px 0 12px",
    },

    checkbox: {
      margin: "16px 0 16px 24px",
      tabletMargin: "auto 0 auto 16px",
    },
  },

  groupButtonsMenu: {
    top: "0",
    background: white,
    boxShadow: " 0px 10px 18px -8px rgba(0, 0, 0, 0.100306)",
    height: "48px",
    tabletHeight: "56px",
    padding: "0 18px 19px 0",
    width: "100%",
    zIndex: "189",
    marginTop: "1px",

    closeButton: {
      right: "11px",
      top: "6px",
      tabletTop: "10px",
      width: "20px",
      height: "20px",
      padding: "8px",
      hoverBackgroundColor: cyanBlueDarkShade,
      backgroundColor: grayMid,
    },
  },

  iconButton: {
    color: gray,
    hoverColor: grayMain,
  },
  selectorAddButton: {
    background: grayLightMid,
    hoverBackground: lightGrayishStrongBlue,
    activeBackground: grayMid,

    iconColor: grayMain,
    iconColorHover: grayMain,
    iconColorActive: grayMain,

    border: `none`,
    boxSizing: "border-box",
    borderRadius: "3px",
    height: " 32px",
    width: "32px",
    padding: "10px",
    color: grayMain,
    hoverColor: black,
  },

  saveCancelButtons: {
    bottom: "0",
    width: "100%",
    left: "0",
    padding: "8px 24px 8px 16px",
    marginRight: "8px",

    unsavedColor: gray,
  },

  selectedItem: {
    background: grayLight,
    border: `1px solid ${globalColors.grayLightMid}`,
    borderRadius: "3px",

    textBox: {
      padding: "0 8px",
      height: "32px",
      alignItems: "center",
      borderRight: `1px solid ${globalColors.grayLightMid}`,
    },

    text: {
      color: cyanBlueDarkShade,
      disabledColor: grayMid,
    },

    closeButton: {
      alignItems: "center",
      padding: "0 8px",
      color: "#979797",
      colorHover: cyanBlueDarkShade,
      backgroundColor: grayLightMid,
    },
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
    hoverIndeterminateColor: black,

    pressedBorderColor: grayMid,
    pressedFillColor: grayLightMid,

    focusColor: gray,

    errorColor: "#F21C0E",
  },

  // slider: {
  //   sliderBarColorProgress: blueMain,
  //   sliderBarColorProgressDisabled: grayMid,
  //   sliderBarColor: grayLightMid,
  //   sliderBarDisableColor: grayLightMid,

  //   sliderBarBorderActive: `1px solid ${globalColors.grayMid}`,
  //   sliderBarBorderDisable: `1px solid ${globalColors.grayMid}`,

  //   thumbFillDisable: grayLightMid,
  //   thumbFillActive: grayLightMid,

  //   thumbBorderColorActive: `1px solid ${globalColors.gray}`,
  //   thumbBorderColorDisable: `1px solid ${globalColors.grayMid}`,

  //   sliderWidth: "202px",

  //   arrowHover: blueMain,
  //   arrowColor: grayMid,
  // },

  viewSelector: {
    fillColor: white,
    checkedFillColor: gray,
    fillColorDisabled: grayLight,
    disabledFillColor: grayLightMid,
    disabledFillColorInner: grayMid,
    hoverBorderColor: gray,
    borderColor: grayMid,
  },

  radioButton: {
    textColor: black,
    textDisableColor: gray,

    marginRight: "8px",

    background: white,
    disableBackground: grayLight,

    fillColor: black,
    borderColor: grayMid,

    disableFillColor: grayMid,
    disableBorderColor: grayLightMid,

    hoverBorderColor: gray,
  },

  requestLoader: {
    backgroundColor: white,
    border: `1px solid ${globalColors.veryLightGrey}`,
    overflow: "hidden",
    padding: "5px 10px",
    lineHeight: "16px",
    borderRadius: "5px",
    boxShadow: "0 2px 8px rgba(0, 0, 0, 0.3)",

    marginRight: "10px",
    top: "10px",
    width: "100%",
  },

  row: {
    minHeight: "47px",
    width: "100%",
    borderBottom: globalColors.grayLightMid,
    backgroundColor: globalColors.lightHover,
    minWidth: "160px",
    overflow: "hidden",
    textOverflow: "ellipsis",

    element: {
      marginRight: "14px",
      marginLeft: "2px",
    },

    optionButton: {
      padding: "8px 9px 9px 7px",
    },
  },

  rowContent: {
    icons: {
      height: "19px",
    },

    margin: "0 6px",
    fontSize: "12px",
    fontStyle: "normal",
    fontWeight: "600",
    height: "56px",
    maxWidth: " 100%",

    sideInfo: {
      minWidth: "160px",
      margin: "0 6px",
      overflow: "hidden",
      textOverflow: "ellipsis",
    },

    mainWrapper: {
      minWidth: "140px",
      marginRight: "8px",
      marginTop: "8px",
      width: "95%",
    },
  },

  badge: {
    border: "1px solid transparent",
    padding: "1px",
    lineHeight: "0.8",
    overflow: "hidden",
    color: white,
    backgroundColor: orangeMain,
    disableBackgroundColor: "#A3A9AE",
  },

  scrollbar: {
    backgroundColorVertical: "rgba(0, 0, 0, 0.1)",
    backgroundColorHorizontal: "rgba(0, 0, 0, 0.1)",
    hoverBackgroundColorVertical: grayMid,
  },

  modalDialog: {
    backgroundColor: white,
    textColor: black,
    headerBorderColor: globalColors.grayLightMid,
    footerBorderColor: globalColors.grayLightMid,
    width: "auto",
    maxwidth: "560px",
    margin: " 0 auto",
    minHeight: "100%",

    colorDisabledFileIcons: "#f3f4f4",

    content: {
      backgroundColor: white,
      modalPadding: "0 12px 12px",
      modalBorderRadius: "6px",
      asidePadding: "0 16px 16px",
      heading: {
        maxWidth: "calc(100% - 18px)",
        margin: "0",
        modalLineHeight: "40px",
        asideLineHeight: "56px",
        fontWeight: "700",
        asideFontSize: "21px",
        modalFontSize: "18px",
      },
    },

    header: {
      borderBottom: `1px solid ${globalColors.lightGrayishBlue}`,
    },

    closeButton: {
      //backgroundColor: "#9a9ea3",
      fillColor: white,
    },
  },

  paging: {
    button: {
      marginRight: "8px",
      maxWidth: "110px",
    },

    page: {
      marginRight: "8px",
      width: "110%",
    },

    comboBox: {
      marginLeft: "auto",
      marginRight: "0px",
    },
  },

  input: {
    color: black,
    disableColor: grayMid,

    backgroundColor: white,
    disableBackgroundColor: grayLight,

    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px",
      large: "550px",
    },

    borderRadius: "3px",
    boxShadow: "none",
    boxSizing: "border-box",
    border: "solid 1px",

    borderColor: grayMid,
    errorBorderColor: "#F21C0E",
    warningBorderColor: warningColor,
    disabledBorderColor: grayLightMid,

    hoverBorderColor: gray,
    hoverErrorBorderColor: red,
    hoverWarningBorderColor: warningColor,
    hoverDisabledBorderColor: grayLightMid,

    focusBorderColor: blueMain,
    focusErrorBorderColor: red,
    focusWarningBorderColor: warningColor,
    focusDisabledBorderColor: grayLightMid,
  },

  fileInput: {
    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px",
      large: "550px",
    },

    height: {
      base: "32px",
      middle: "38px",
      big: "38px",
      huge: "39px",
      large: "44px",
    },

    paddingRight: {
      base: "37px",
      middle: "48px",
      big: "53px",
      huge: "58px",
      large: "64px",
    },

    icon: {
      background: white,

      border: "1px solid",
      borderRadius: "0 3px 3px 0",

      width: {
        base: "30px",
        middle: "36px",
        big: "37px",
        huge: "38px",
        large: "48px",
      },

      height: {
        base: "30px",
        middle: "36px",
        big: "36px",
        huge: "37px",
        large: "42px",
      },
    },

    iconButton: {
      width: {
        base: "15px",
        middle: "15px",
        big: "16px",
        huge: "16px",
        large: "16px",
      },
    },
  },

  passwordInput: {
    disableColor: grayMid,
    color: gray,

    iconColor: grayMid,
    hoverIconColor: gray,

    hoverColor: gray,

    lineHeight: "32px",

    tooltipTextColor: black,

    text: {
      lineHeight: "14px",
      marginTop: "-2px",
    },

    link: {
      marginTop: "-6px",

      tablet: {
        width: "100%",
        marginLeft: "0px",
        marginTop: "-1px",
      },
    },

    progress: {
      borderRadius: "2px",
      marginTop: "-2px",
    },

    newPassword: {
      margin: "0 16px",

      svg: {
        overflow: "hidden",
        marginBottom: "4px",
      },
    },
  },

  searchInput: {
    fontSize: "14px",
    fontWeight: "600",

    iconColor: grayMid,
    hoverIconColor: grayMid,
  },

  inputPhone: {
    activeBorderColor: "#2da7db",
    inactiveBorderColor: "#d0d5da",
    errorBorderColor: "#f21c0e",
    backgroundColor: "#fff",
    color: "#33333",
    scrollBackground: "#a3a9ae",
    placeholderColor: "#a3a9ae",
    dialCodeColor: "#a3a9ae",
    width: "320px",
    height: "44px",
  },

  textInput: {
    fontWeight: "normal",
    placeholderColor: gray,
    disablePlaceholderColor: grayMid,

    transition: "all 0.2s ease 0s",
    appearance: "none",
    display: "flex",
    flex: "1 1 0%",
    outline: "none",
    overflow: "hidden",
    opacity: "1",

    lineHeight: {
      base: "20px",
      middle: "20px",
      big: "20px",
      huge: "21px",
      large: "20px",
    },

    fontSize: {
      base: "13px",
      middle: "14px",
      big: "16px",
      huge: "18px",
      large: "16px",
    },

    padding: {
      base: "5px 6px",
      middle: "8px 12px",
      big: "8px 16px",
      huge: "8px 20px",
      large: "11px 12px",
    },
  },

  inputBlock: {
    height: "100%",
    paddingRight: "8px",
    paddingLeft: "1px",

    display: "flex",
    alignItems: "center",
    padding: "2px 0px 2px 2px",
    margin: "0",

    borderColor: blueMain,

    iconColor: grayMid,
    hoverIconColor: grayMid,
  },

  textArea: {
    disabledColor: grayLight,

    focusBorderColor: blueMain,
    focusErrorBorderColor: red,
    focusOutline: "none",

    scrollWidth: "100%",
    scrollHeight: "91px",

    numerationColor: "#A3A9AE",

    copyIconFilter:
      "invert(71%) sepia(1%) saturate(1597%) hue-rotate(166deg) brightness(100%) contrast(73%)",
  },

  link: {
    color: black,
    lineHeight: "calc(100% + 6px)",
    opacity: "0.5",
    textDecoration: "none",
    cursor: "pointer",
    display: "inline-block",

    hover: {
      textDecoration: "underline dashed",
      page: { textDecoration: "underline" },
    },
  },

  linkWithDropdown: {
    paddingRight: "20px",
    semiTransparentOpacity: "0.5",
    textDecoration: "none",
    disableColor: gray,

    svg: {
      opacity: "1",
      semiTransparentOpacity: "0.5",
    },

    text: { maxWidth: "100%" },

    span: { maxWidth: "300px" },

    expander: {
      iconColor: black,
    },

    color: {
      default: "#A3A9AE",
      hover: "#555F65",
      active: "#333333",
      focus: "#333333",
    },

    background: {
      default: "transparent",
      hover: "#ECEEF1",
      active: "#D0D5DA",
      focus: "#DFE2E3",
    },

    caret: {
      width: "5px",
      minWidth: "5px",
      height: "4px",
      minHeight: "4px",
      marginLeft: "5px",
      marginTop: "-4px",
      right: "6px",
      top: "0",
      bottom: "0",
      isOpenBottom: "-1px",
      margin: "auto",
      opacity: "0",
      transform: "scale(1, -1)",
    },
  },

  tooltip: {
    borderRadius: "6px",
    boxShadow: "0px 10px 15px rgba(4, 15, 27, 0.13)",
    opacity: "1",
    padding: "8px 12px",
    pointerEvents: "auto",
    maxWidth: "340px",
    color: "#F8F7BF",
    textColor: black,

    before: {
      border: "none",
    },
    after: {
      border: "none",
    },
  },

  tabsContainer: {
    scrollbar: {
      width: "100%",
      height: "44px",
    },

    label: {
      height: " 32px",
      borderRadius: "16px",
      minWidth: "fit-content",
      marginRight: "8px",
      width: "fit-content",

      backgroundColor: blueLightMid,
      hoverBackgroundColor: grayLight,
      disableBackgroundColor: grayLightMid,

      title: {
        margin: "7px 15px 7px 15px",
        overflow: "hidden",
        color: white,
        hoverColor: black,
        disableColor: grayMid,
      },
    },
  },

  fieldContainer: {
    horizontal: {
      margin: "0 0 16px 0",

      label: {
        lineHeight: "32px",
        margin: "0",
      },

      body: {
        flexGrow: "1",
      },

      iconButton: {
        marginTop: "10px",
        marginLeft: "8px",
      },
    },

    vertical: {
      margin: "0 0 16px 0",

      label: {
        lineHeight: "20px",
        height: "20px",
      },

      labelIcon: {
        width: "100%",
        margin: "0 0 4px 0",
      },

      body: {
        width: "100%",
      },

      iconButton: {
        margin: "0",
        padding: "0px 8px",
        width: "12px",
        height: "12px",
      },
    },

    errorLabel: {
      color: "#F21C0E",
    },
  },

  avatar: {
    defaultImage: `url("${AvatarBaseReactSvgUrl}")`,
    initialsContainer: {
      color: white,
      left: "50%",
      top: "50%",
      transform: "translate(-50%, -50%)",
      fontWeight: "600",

      fontSize: {
        min: "12px",
        small: "12px",
        base: "16px",
        medium: "20px",
        big: "34px",
        max: "72px",
      },
    },

    roleWrapperContainer: {
      right: {
        min: "-5px",
        small: "-2px",
        base: "-2px",
        medium: "-4px",
        big: "3px",
        max: "0px",
      },

      bottom: {
        min: "-5px",
        small: "3px",
        base: "4px",
        medium: "6px",
        big: "3px",
        max: "0px",
      },

      width: {
        medium: "16px",
        max: "24px",
      },

      height: {
        medium: "16px",
        max: "24px",
      },
    },

    imageContainer: {
      backgroundImage: blueMain,
      background: grayMid,
      borderRadius: "50%",
      height: "100%",

      svg: {
        display: "block",
        width: "50%",
        height: "100%",
        margin: "auto",
        fill: white,
      },
    },

    administrator: {
      fill: orangeMain,
      stroke: darkBlack,
      color: white,
    },

    guest: {
      fill: "#3B72A7",
      stroke: darkBlack,
      color: white,
    },

    owner: {
      fill: "#EDC409",
      stroke: darkBlack,
      color: white,
    },

    editContainer: {
      right: "0px",
      bottom: "0px",
      fill: white,
      backgroundColor: blueLightMid,
      borderRadius: "50%",
      height: "32px",
      width: "32px",
    },

    image: {
      width: "100%",
      height: "100%",
      borderRadius: "50%",
    },

    icon: {
      background: "#ECEEF1",
      color: "#A3A9AE",
    },

    width: {
      min: "32px",
      small: "36px",
      base: "40px",
      medium: "48px",
      big: "80px",
      max: "124px",
    },

    height: {
      min: "32px",
      small: "36px",
      base: "40px",
      medium: "48px",
      big: "80px",
      max: "124px",
    },
  },

  avatarEditor: {
    minWidth: "208px",
    maxWidth: "300px",
    width: "max-content",
  },

  avatarEditorBody: {
    maxWidth: "400px",

    selectLink: {
      color: black,
      linkColor: link,
    },

    slider: {
      width: "100%",
      margin: "24px 0",
      backgroundColor: "transparent",

      runnableTrack: {
        background: grayLightMid,
        focusBackground: grayLightMid,
        border: `1.4px solid ${grayLightMid}`,
        borderRadius: "5.6px",
        width: "100%",
        height: "8px",
      },

      sliderThumb: {
        marginTop: "-9.4px",
        width: "24px",
        height: "24px",
        background: blueMain,
        disabledBackground: "#A6DCF2",
        borderWidth: "6px",
        borderStyle: "solid",
        borderColor: `${white}`,
        borderRadius: "30px",
        boxShadow: "0px 5px 20px rgba(4, 15, 27, 0.13)",
      },

      thumb: {
        width: "24px",
        height: "24px",
        background: blueMain,
        border: `6px solid ${white}`,
        borderRadius: "30px",
        marginTop: "0px",
        boxShadow: "0px 5px 20px rgba(4, 15, 27, 0.13)",
      },

      rangeTrack: {
        background: grayLightMid,
        border: `1.4px solid ${grayLightMid}`,
        borderRadius: "5.6px",
        width: "100%",
        height: "8px",
      },

      rangeThumb: {
        width: "14px",
        height: "14px",
        background: blueMain,
        border: `6px solid ${white}`,
        borderRadius: "30px",
        boxShadow: "0px 5px 20px rgba(4, 15, 27, 0.13)",
      },

      track: {
        background: "transparent",
        borderColor: "transparent",
        borderWidth: "10.2px 0",
        color: "transparent",
        width: "100%",
        height: "8px",
      },

      trackNumber: {
        color: "#A3A9AE",
      },

      fillLower: {
        background: grayLightMid,
        focusBackground: grayLightMid,
        border: `1.4px solid ${grayLightMid}`,
        borderRadius: "11.2px",
      },

      fillUpper: {
        background: grayLightMid,
        focusBackground: grayLightMid,
        border: `1.4px solid ${grayLightMid}`,
        borderRadius: "11.2px",
      },
    },

    dropZone: {
      border: `1px dashed ${silver}`,
    },

    container: {
      miniPreview: {
        width: "160px",
        border: `1px solid ${grayLightMid}`,
        borderRadius: "6px",
        padding: "8px",
      },

      buttons: {
        height: "32px",
        background: gray,

        mobileWidth: "40px",
        mobileHeight: "100%",
        mobileBackground: "none",
      },

      button: {
        background: gray,
        fill: white,
        hoverFill: white,
        padding: "0 12px",
        height: "40px",
        borderRadius: "6px",
      },

      zoom: {
        height: "56px",

        mobileHeight: "24px",
        marginTop: "16px",
      },
    },
  },

  backdrop: {
    backgroundColor: "rgba(6, 22, 38, 0.2)",
    unsetBackgroundColor: "unset",
  },

  treeMenu: {
    disabledColor: "#767676",
  },

  treeNode: {
    background: "#f3f4f4",
    disableColor: "#A3A9AE",

    dragging: {
      draggable: {
        background: lightCumulus,
        hoverBackgroundColor: lightMediumGoldenrod,
        borderRadius: "3px",
      },

      title: {
        width: "85%",
      },
    },
    icon: {
      color: grayMain,
    },

    draggable: {
      color: cyanBlueDarkShade,
      dragOverBackgroundColor: strongBlue,
      border: `1px ${strongBlue} solid`,
      dragOverColor: white,

      gapTop: {
        borderTop: `2px blue solid`,
      },

      gapBottom: {
        borderBottom: `2px blue solid`,
      },
    },

    contentWrapper: {
      color: darkRed,
    },

    title: {
      color: cyanBlueDarkShade,
    },

    selected: {
      background: lightGrayishStrongBlue,
      hoverBackgroundColor: lightGrayishStrongBlue,
      borderRadius: "3px",
    },

    checkbox: {
      border: `2px solid ${white}`,
      borderTop: 0,
      borderLeft: 0,
    },
  },

  progressBar: {
    height: "22px",
    backgroundColor: grayLight,
    marginLeft: "-100%",

    fullText: {
      padding: "0px 6px",
      fontWeight: "600",
      margin: "0",
    },

    percent: {
      float: "left",
      overflow: "hidden",
      maxHeight: "22px",
      minHeight: "22px",
      transition: "width 0.6s ease",
      background: "linear-gradient(90deg, #20d21f 75%, #b9d21f 100%)",
    },

    text: {
      minWidth: "200%",

      progressText: {
        padding: "2px 6px",
        margin: "0",
        minWidth: "100px",
        fontWeight: "600",
      },
    },

    dropDown: {
      padding: "16px 16px 16px 17px",
    },
  },

  dropDown: {
    fontWeight: "600",
    fontSize: "13px",
    zIndex: "400",
    background: white,
    borderRadius: "6px",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    border: "none",
  },

  dropDownItem: {
    color: black,
    disableColor: gray,
    backgroundColor: white,
    hoverBackgroundColor: grayLight,
    hoverDisabledBackgroundColor: white,
    selectedBackgroundColor: lightHover,
    fontWeight: "600",
    fontSize: "13px",
    width: "100%",
    maxWidth: "500px",
    border: "0px",
    margin: "0px",
    padding: "0px 12px",
    tabletPadding: "0px 16px",
    lineHeight: "32px",
    tabletLineHeight: "36px",

    icon: {
      width: "16px",
      marginRight: "8px",
      lineHeight: "10px",

      color: black,
      disableColor: gray,
    },

    separator: {
      padding: "0px 16px",
      borderBottom: `1px solid ${globalColors.grayLightMid}`,
      margin: " 4px 16px 4px",
      lineHeight: "1px",
      height: "1px",
      width: "calc(100% - 32px)",
    },
  },

  toast: {
    active: {
      success: activeSuccess,
      error: activeError,
      info: activeInfo,
      warning: activeWarning,
    },
    hover: {
      success: hoverSuccess,
      error: hoverError,
      info: hoverInfo,
      warning: hoverWarning,
    },
    border: {
      success: "none",
      error: "none",
      info: "none",
      warning: "none",
    },

    zIndex: "9999",
    position: "fixed",
    padding: "4px",
    width: "320px",
    color: white,
    top: "16px",
    right: "24px",
    marginTop: "0px",

    closeButton: {
      color: white,
      fontWeight: "700",
      fontSize: "14px",
      background: "transparent",
      padding: "0",
      opacity: "0.7",
      hoverOpacity: "1",
      transition: "0.3s ease",
    },

    main: {
      marginBottom: "1rem",
      boxShadow: "0px 10px 16px -12px rgba(0, 0, 0, 0.3)",
      maxHeight: "800px",
      overflow: "hidden",
      borderRadius: "6px",
      color: darkBlack,
      margin: "0 0 12px",
      padding: "12px",
      minHeight: "32px",
      width: "100%",
      right: "0",
      transition: "0.3s",
    },
  },

  toastr: {
    svg: {
      width: "16px",
      minWidth: "16px",
      height: "16px",
      minHeight: "16px",
      color: {
        success: black,
        error: black,
        info: black,
        warning: black,
      },
    },

    text: {
      lineHeight: " 1.3",
      fontSize: "12px",
      color: black,
    },

    title: {
      fontWeight: "600",
      margin: "0",
      marginBottom: "5px",
      lineHeight: "16px",
      color: {
        success: darkBlack,
        error: darkBlack,
        info: darkBlack,
        warning: darkBlack,
      },
      fontSize: "12px",
    },

    closeButtonColor: black,
  },

  loader: {
    color: shuttleGrey,
    size: "40px",
    marginRight: "2px",
    borderRadius: "50%",
  },

  rombsLoader: {
    blue: {
      colorStep_1: "#F2CBBF",
      colorStep_2: "#fff",
      colorStep_3: "#E6E4E4",
      colorStep_4: "#D2D2D2",
    },
    red: {
      colorStep_1: "#BFE8F8",
      colorStep_2: "#fff",
      colorStep_3: "#EFEFEF",
    },
    green: {
      colorStep_1: "#CBE0AC",
      colorStep_2: "#fff",
      colorStep_3: "#EFEFEF",
      colorStep_4: "#E6E4E4",
    },
  },

  dialogLoader: {
    borderBottom: "1px solid rgb(222, 226, 230)",
  },

  // dropDownItem: {
  //   width: "100%",
  //   maxWidth: "240px",
  //   border: "none",
  //   cursor: "pointer",
  //   padding: "0px 16px",
  //   lineHeight: "32px",
  //   textAlign: "left",
  //   background: "none",
  //   textDecoration: "none",
  //   fontStyle: "normal",
  //   fontWeight: "600",
  //   fontSize: "13px",

  //   whiteSpace: "nowrap",
  //   overflow: "hidden",
  //   textOverflow: "ellipsis",

  //   outline: "none",
  //   color: black,
  //   textTransform: "none",

  //   hoverBackgroundColor: grayLight,
  //   noHoverBackgroundColor: white,

  //   header: {
  //     color: gray,
  //     hoverCursor: "default",
  //     hoverBackgroundColor: "white",
  //     textTransform: "uppercase",
  //   },

  //   disabled: {
  //     color: gray,
  //     hoverCursor: "default",
  //     hoverBackgroundColor: "white",
  //   },

  //   separator: {
  //     padding: "0px 16px",
  //     border: `0.5px solid ${grayLightMid}`,
  //     cursor: "default",
  //     margin: "6px 16px 6px",
  //     lineHeight: "1px",
  //     height: "1px",
  //     width: "calc(100% - 32px)",
  //   },

  //   tablet: { lineHeight: "36px" },

  comboBox: {
    padding: "6px 0px",
    background: lightGrayishStrongBlue,

    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px",
    },

    arrow: {
      width: "6px",
      flex: "0 0 6px",
      marginTopWithBorder: "5px",
      marginTop: "12px",
      marginRight: "8px",
      marginLeft: "auto",
    },

    button: {
      height: "18px",
      heightWithBorder: "30px",
      heightModernView: "28px",

      paddingLeft: "16px",
      paddingRightNoArrow: "16px",
      paddingRight: "8px",

      selectPaddingLeft: "8px",
      selectPaddingRightNoArrow: "14px",
      selectPaddingRight: "6px",

      color: black,
      disabledColor: grayMid,
      background: white,
      backgroundWithBorder: "none",
      backgroundModernView: "none",

      border: `1px solid ${grayMid}`,
      borderRadius: "3px",
      borderColor: blueMain,
      openBorderColor: blueMain,
      disabledBorderColor: grayLightMid,
      disabledBackground: grayLight,

      hoverBorderColor: gray,
      hoverBorderColorOpen: blueMain,
      hoverDisabledBorderColor: grayLightMid,

      hoverBackgroundModernView: "#ECEEF1",
      activeBackgroundModernView: "#D0D5DA",
      focusBackgroundModernView: "#DFE2E3",
    },

    label: {
      marginRightWithBorder: "8px",
      marginRight: "4px",

      disabledColor: grayMid,
      color: black,
      selectedColor: black,
      maxWidth: "175px",

      lineHeightWithoutBorder: "16px",
      lineHeightTextDecoration: "underline dashed",
    },

    childrenButton: {
      marginRight: "8px",
      width: "16px",
      height: "16px",

      defaultDisabledColor: grayMid,
      defaultColor: gray,
      disabledColor: grayMid,
      color: black,
    },
  },

  toggleContent: {
    headingHeight: "24px",
    headingLineHeight: "26px",
    hoverBorderBottom: "1px dashed",
    contentPadding: "10px 0px 0px 0px",
    arrowMargin: "4px 8px 4px 0px",
    transform: "rotate(180deg)",
    iconColor: black,

    childrenContent: {
      color: black,
      paddingTop: "6px",
    },
  },

  toggleButton: {
    fillColorDefault: "#4781D1",
    fillColorOff: "#D0D5DA",
    hoverFillColorOff: "#A3A9AE",
    fillCircleColor: white,
    fillCircleColorOff: white,
  },

  contextMenuButton: {
    content: {
      width: "100%",
      backgroundColor: white,
      padding: "0 16px 16px",
    },

    headerContent: {
      maxWidth: "500px",
      margin: "0",
      lineHeight: "56px",
      fontWeight: "700",
      borderBottom: `1px solid ${globalColors.lightGrayishBlue}`,
    },

    bodyContent: {
      padding: "16px 0",
    },
  },

  calendar: {
    color: "#333333",
    disabledColor: "#DFE2E3",
    pastColor: "#A3A9AE",
    onHoverBackground: "#f3f4f4",
    titleColor: "#555F65",
    outlineColor: "#eceef1",
    arrowColor: "#555f65",
    disabledArrow: "#A3A9AE",
    weekdayColor: "#A3A9AE",
    accent: "#4781d1",
  },

  datePicker: {
    width: "115px",
    dropDownPadding: "16px 16px 16px 17px",
    contentPadding: "0 16px 16px",
    bodyPadding: "16px 0",
    backgroundColor: white,
    inputBorder: blueMain,
    iconPadding: "8px 8px 7px 0px",

    contentMaxWidth: "500px",
    contentLineHeight: "56px",
    contentFontWeight: "700",

    borderBottom: `1px solid ${globalColors.lightGrayishBlue}`,
  },

  aside: {
    backgroundColor: white,
    height: "100%",
    overflowX: "hidden",
    overflowY: "auto",
    position: "fixed",
    right: "0",
    top: "0",
    bottom: "16px",
    paddingBottom: "64px",
    transition: "transform 0.3s ease-in-out",
  },

  dragAndDrop: {
    height: "100%",
    border: `1px solid ${globalColors.darkSilver}`,
    transparentBorder: "1px solid transparent",
    acceptBackground: lightMediumGoldenrod,
    background: lightCumulus,
  },

  // phoneInput: {
  //   width: "304px",
  //   height: "44px",
  //   itemTextColor: black,
  //   itemBackgroundColor: white,
  //   itemHoverColor: grayLightMid,
  //   scrollBackground: "rgba(0, 0, 0, 0.1)",
  //   placeholderColor: gray,
  // },

  // squareButton: {
  //   height: "32px",
  //   width: "32px",
  //   color: gray,
  //   backgroundColor: white,
  //   border: `1px solid ${grayMid}`,
  //   borderRadius: "3px",
  //   outline: "none",
  //   hover: {
  //     backgroundColor: white,
  //     border: `1px solid ${gray}`,
  //   },
  //   click: {
  //     backgroundColor: grayLightMid,
  //     border: `1px solid ${gray}`,
  //   },
  //   disable: {
  //     backgroundColor: grayLight,
  //     border: `1px solid ${grayLightMid}`,
  //   },
  //   crossShape: {
  //     color: graySilver,
  //     disable: {
  //       color: gray,
  //     },
  //   },
  // },

  // roundButton: {
  //   height: "40px",
  //   width: "40px",
  //   backgroundColor: grayLight,
  //   borderRadius: {
  //     plus: "112px",
  //     minus: "81px",
  //   },
  //   borderStyle: "none",
  //   outline: "none",
  //   hover: {
  //     backgroundColor: grayLightMid,
  //   },
  //   click: {
  //     backgroundColor: grayMid,
  //   },
  //   disable: {
  //     backgroundColor: grayLight,
  //   },
  //   plus: {
  //     color: grayMid,
  //     disable: {
  //       color: black,
  //     },
  //   },
  // },

  catalog: {
    background: "#f8f9f9",

    header: {
      borderBottom: "1px solid #eceef1",
      iconFill: "#657077",
    },
    control: {
      background: "#9a9ea3",
      fill: "#ffffff",
    },

    headerBurgerColor: "#657077",

    verticalLine: "1px solid #eceef1",

    profile: {
      borderTop: "1px solid #eceef1",
      background: "#f3f4f4",
    },

    paymentAlert: {
      color: "#ed7309",
      warningColor: "#F21C0E",
    },

    teamTrainingAlert: {
      titleColor: "#388BDE",
      borderColor: "#388BDE",
      linkColor: "#5299E0",
    },
  },

  alertComponent: {
    descriptionColor: "#555F65",
    iconColor: "#657077",
  },

  catalogItem: {
    container: {
      width: "100%",
      height: "36px",
      padding: "0 12px",
      marginBottom: "16px",
      background: "#fff",
      tablet: {
        height: "44px",
        padding: "0 12px",
        marginBottom: "24px",
      },
    },
    sibling: {
      active: {
        background: lightGrayishStrongBlue,
      },
      hover: {
        background: grayLightMid,
      },
    },
    img: {
      svg: {
        width: "16px",
        height: "16px",

        fill: "#657077",
        isActiveFill: "#4781D1",
        tablet: {
          width: "20px",
          height: "20px",
        },
      },
    },
    text: {
      width: "100%",
      marginLeft: "8px",
      lineHeight: "20px",
      color: cyanBlueDarkShade,
      isActiveColor: "#4781D1",
      fontSize: "13px",
      fontWeight: 600,
      tablet: {
        marginLeft: "12px",
        lineHeight: "20px",
        fontSize: "15px",
        fontWeight: "600",
      },
    },
    initialText: {
      color: white,
      width: "16px",
      lineHeight: "11px",
      fontSize: "11px",
      fontWeight: "bold",
      tablet: {
        width: "20px",
        lineHeight: "19px",
        fontSize: "11px",
      },
    },
    badgeWrapper: {
      size: "16px",
      marginLeft: "8px",
      marginRight: "-2px",
      tablet: {
        width: "44px",
        height: "44px",
        marginRight: "-16px",
      },
    },
    badgeWithoutText: {
      backgroundColor: orangeMain,

      size: "8px",
      position: "-4px",
    },
    trashIconFill: "#A3A9AE",
  },

  navigation: {
    expanderColor: black,
    background: white,
    rootFolderTitleColor: "#A3A9AE",

    icon: {
      fill: "#316DAA",
      stroke: "#DFE2E3",
    },
  },

  nav: {
    backgroundColor: "#0F4071",
  },

  navItem: {
    baseColor: "#7A95B0",
    activeColor: white,
    separatorColor: "#3E668D",

    wrapper: {
      hoverBackground: "#0d3760",
    },
  },

  header: {
    backgroundColor: "#F8F9F9",
    recoveryColor: "#657077",
    linkColor: "#657077",
    productColor: white,
  },

  menuContainer: {
    background: "#F3F4F4",
    color: black,
  },

  article: {
    background: grayLight,
    pinBorderColor: grayLightMid,
    catalogItemHeader: "#A3A9AE",
    catalogItemText: "#555F65",
    catalogItemActiveBackground: "#DFE2E3",
    catalogShowText: "#657077",
  },

  section: {
    toggler: {
      background: white,
      fill: gray,
      boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    },

    header: {
      backgroundColor: white,
      background: `linear-gradient(180deg,#ffffff 2.81%,rgba(255, 255, 255, 0.91) 63.03%,rgba(255, 255, 255, 0) 100%)`,
      trashErasureLabelBackground: "#f8f9f9",
      trashErasureLabelText: "#555f65",
    },
  },

  infoPanel: {
    sectionHeaderToggleIcon: gray,
    sectionHeaderToggleIconActive: "#3B72A7",
    sectionHeaderToggleBg: "transparent",
    sectionHeaderToggleBgActive: grayLight,

    backgroundColor: white,
    blurColor: "rgba(6, 22, 38, 0.2)",
    borderColor: grayLightMid,
    thumbnailBorderColor: grayLightMid,
    textColor: black,

    closeButtonWrapperPadding: "0px",
    closeButtonIcon: white,
    closeButtonSize: "17px",
    closeButtonBg: "transparent",

    members: {
      iconColor: "#A3A9AE",
      iconHoverColor: "#657077",
      isExpectName: "#A3A9AE",
      subtitleColor: "#a3a9ae",
      meLabelColor: "#a3a9ae",
      roleSelectorColor: "#a3a9ae",
      disabledRoleSelectorColor: "#a3a9ae",
      roleSelectorArrowColor: "#a3a9ae",
    },

    history: {
      subtitleColor: "#a3a9ae",
      fileBlockBg: "#f8f9f9",
      dateColor: "#A3A9AE",
      fileExstColor: "#A3A9AE",
      locationIconColor: "#A3A9AE",
      folderLabelColor: "#A3A9AE",
    },

    details: {
      customLogoBorderColor: grayLightMid,
      commentEditorIconColor: "#333",
      tagBackground: "#ECEEF1",
    },

    gallery: {
      borderColor: "#d0d5da",
    },
  },

  filesArticleBody: {
    background: lightGrayishStrongBlue,
    panelBackground: lightGrayishStrongBlue,

    fill: grayMain,
    expanderColor: "dimgray",
    downloadAppList: {
      color: "#83888d",
      winHoverColor: "#3785D3",
      macHoverColor: "#000",
      linuxHoverColor: "#FFB800",
      androidHoverColor: "#9BD71C",
    },
    thirdPartyList: {
      color: "#818b91",
      linkColor: cyanBlueDarkShade,
    },
  },

  peopleArticleBody: {
    iconColor: grayMain,
    expanderColor: "dimgray",
  },

  peopleTableRow: {
    fill: "#3b72a7",

    nameColor: black,
    pendingNameColor: gray,

    sideInfoColor: gray,
    pendingSideInfoColor: grayMid,
  },

  filterInput: {
    button: {
      border: "1px solid #d0d5da",
      hoverBorder: "1px solid #a3a9ae",

      openBackground: "#a3a9ae",

      openFill: "#ffffff",
    },

    filter: {
      background: "#ffffff",
      border: "1px solid #eceef1",
      color: "#a3a9ae",

      separatorColor: "#eceef1",
      indicatorColor: "#ED7309",

      selectedItem: {
        background: "#265a8f",
        border: "#265a8f",
        color: "#ffffff",
      },
    },

    sort: {
      background: "#ffffff",
      hoverBackground: "#f8f9f9",
      selectedViewIcon: "#dfe2e3",
      viewIcon: "#a3a9ae",
      sortFill: "#657077",

      tileSortFill: black,
      tileSortColor: black,
    },

    selectedItems: {
      background: "#eceef1",
      hoverBackground: "#F3F4F4",
    },
  },

  profileInfo: {
    color: "#83888d",
    iconButtonColor: black,
    linkColor: gray,

    tooltipLinkColor: black,
    iconColor: "#C96C27",
  },

  updateUserForm: {
    tooltipTextColor: black,
    borderTop: "1px solid #eceef1",
  },

  tableContainer: {
    borderRight: `2px solid ${grayMid}`,
    hoverBorderColor: grayMain,
    tableCellBorder: `1px solid ${grayLightMid}`,

    groupMenu: {
      background: white,
      borderBottom: "1px solid transparent",
      borderRight: `1px solid ${grayMid}`,
      boxShadow: "0px 5px 20px rgba(4, 15, 27, 7%)",
    },

    header: {
      background: white,
      borderBottom: `1px solid ${grayLightMid}`,
      textColor: gray,
      activeTextColor: grayMain,
      hoverTextColor: grayMain,

      iconColor: gray,
      activeIconColor: grayMain,
      hoverIconColor: grayMain,

      borderImageSource: `linear-gradient(to right,${white} 21px,${grayLightMid} 21px,${grayLightMid} calc(100% - 20px),${white} calc(100% - 20px))`,
      lengthenBorderImageSource: `linear-gradient(to right, ${grayLightMid}, ${grayLightMid})`,
      hotkeyBorderBottom: `1px solid ${globalColors.blueMain}`,

      settingsIconDisableColor: "#D0D5DA",
    },

    tableCell: {
      border: `1px solid ${grayLightMid}`,
    },
  },

  filesSection: {
    rowView: {
      checkedBackground: "#f3f4f4",

      draggingBackground: lightCumulus,
      draggingHoverBackground: lightMediumGoldenrod,

      shareButton: {
        color: grayMain,
        fill: grayMain,
      },

      sideColor: gray,
      linkColor: black,
      textColor: gray,

      editingIconColor: "#3b72a7",
      shareHoverColor: "#3b72a7",
      pinColor: "#3b72a7",
    },

    tableView: {
      fileName: {
        linkColor: black,
        textColor: gray,
      },

      row: {
        checkboxChecked: `linear-gradient(to right, #f3f4f4 24px, ${grayLightMid} 24px)`,
        checkboxDragging: `linear-gradient(to right, ${lightCumulus} 24px, ${grayLightMid} 24px)`,
        checkboxDraggingHover: `linear-gradient(to right,rgb(239, 239, 178) 24px, ${grayLightMid} 24px)`,

        contextMenuWrapperChecked: `linear-gradient(to left, #f3f4f4 24px, ${grayLightMid} 24px)`,
        contextMenuWrapperDragging: `border-image-source: linear-gradient(to left, ${lightCumulus} 24px, ${grayLightMid} 24px)`,
        contextMenuWrapperDraggingHover: `linear-gradient(to left,rgb(239, 239, 178) 24px,${grayLightMid} 24px)`,

        backgroundActive: `#F3F4F4`,

        borderImageCheckbox: `linear-gradient(to right, ${white} 24px, ${grayLightMid} 24px)`,
        borderImageContextMenu: `linear-gradient(to left, ${white} 24px, ${grayLightMid} 24px)`,

        borderHover: gray,
        sideColor: gray,
        shareHoverColor: "#3b72a7",

        borderImageRight:
          "linear-gradient(to right, #ffffff 25px,#eceef1 24px)",
        borderImageLeft: "linear-gradient(to left, #ffffff 24px,#eceef1 24px)",

        borderColor: "#ECEEf1",
        borderColorTransition: "#f3f4f4",
      },
    },

    tilesView: {
      tile: {
        draggingColor: lightCumulus,
        draggingHoverColor: lightMediumGoldenrod,
        checkedColor: "#f3f4f4",
        roomsCheckedColor: "#f3f4f4",
        border: `1px solid ${grayMid}`,
        backgroundBadgeColor: white,
        backgroundColor: white,
        borderRadius: "6px",
        roomsBorderRadius: "12px",
        bottomBorderRadius: "0 0 6px 6px",
        roomsBottomBorderRadius: "0 0 12px 12px",
        upperBorderRadius: "6px 6px 0 0",
        roomsUpperBorderRadius: "12px 12px 0 0",
        backgroundColorTop: white,
      },

      sideColor: black,
      color: black,
      textColor: gray,
    },

    animationColor: "rgba(82, 153, 224, 0.16)",
  },

  advancedSelector: {
    footerBorder: `1px solid ${grayLightMid}`,

    hoverBackgroundColor: grayLightMid,
    selectedBackgroundColor: grayLightMid,
    borderLeft: `1px solid ${grayLightMid}`,

    searcher: {
      hoverBorderColor: grayMid,
      focusBorderColor: blueMain,
      placeholderColor: gray,
    },
  },

  selector: {
    border: `1px solid ${grayLightMid}`,

    breadCrumbs: {
      prevItemColor: "#A3A9AE",
      arrowRightColor: "#A3A9AE",
    },

    bodyDescriptionText: "#A3A9AE",

    item: {
      hoverBackground: grayLight,
      selectedBackground: lightHover,
    },

    emptyScreen: {
      descriptionColor: cyanBlueDarkShade,
    },
  },

  floatingButton: {
    backgroundColor: "#3B72A7",
    color: white,
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    fill: white,

    alert: {
      fill: "",
      path: "",
    },
  },

  mediaViewer: {
    color: "#d1d1d1",
    background: "rgba(17, 17, 17, 0.867)",
    backgroundColor: "rgba(11, 11, 11, 0.7)",
    fill: white,
    titleColor: white,
    iconColor: white,

    controlBtn: {
      backgroundColor: "rgba(200, 200, 200, 0.2)",
    },

    imageViewer: {
      backgroundColor: "rgba(200, 200, 200, 0.2)",
      inactiveBackgroundColor: "rgba(11,11,11,0.7)",
      fill: white,
    },

    progressBar: {
      background: "#d1d1d1",
      backgroundColor: "rgba(200, 200, 200, 0.2)",
    },

    scrollButton: {
      backgroundColor: "rgba(11, 11, 11, 0.7)",
      background: "rgba(200, 200, 200, 0.2)",
      border: `solid ${white}`,
    },

    videoViewer: {
      fill: white,
      stroke: white,
      color: "#d1d1d1",
      colorError: white,
      backgroundColorError: darkBlack,
      backgroundColor: "rgba(11, 11, 11, 0.7)",
      background: "rgba(200, 200, 200, 0.2)",
    },
  },

  connectCloud: {
    connectBtnContent: black,
    connectBtnTextBg: white,
    connectBtnIconBg: white,
    connectBtnTextBorder: grayMid,
    connectBtnIconBorder: grayMid,
  },

  createEditRoomDialog: {
    commonParam: {
      descriptionColor: "#a3a9ae",
    },

    roomType: {
      listItem: {
        background: "none",
        borderColor: "#ECEEF1",
        descriptionText: "#A3A9AE",
      },
      dropdownButton: {
        background: "none",
        borderColor: "#ECEEF1",
        isOpenBorderColor: "#2DA7DB",
        descriptionText: "#A3A9AE",
      },
      dropdownItem: {
        background: "#ffffff",
        hoverBackground: "#f3f4f4",
        descriptionText: "#A3A9AE",
      },
      displayItem: {
        background: "#f8f8f8",
        borderColor: "#f8f8f8",
        descriptionText: "#555F65",
      },
    },

    roomTypeDropdown: {
      desktop: {
        background: "#ffffff",
        borderColor: "#d0d5da",
      },
      mobile: {
        background: "#ffffff",
      },
    },

    permanentSettings: {
      background: "#f8f9f9",
      isPrivateIcon: "#35ad17",
      descriptionColor: "#555f65",
    },

    tagInput: {
      tagBackground: "#ECEEF1",
      tagHoverBackground: "#F3F4F4",
    },

    dropdown: {
      background: "#ffffff",
      borderColor: "#d0d5da",
      item: {
        hoverBackground: "#f3f4f4",
      },
    },

    isPrivate: {
      limitations: {
        background: "#f8f9f9",
        iconColor: "#ed7309",
        titleColor: "#ed7309",
        descriptionColor: "#555f65",
        linkColor: "#555f65",
      },
    },

    thirdpartyStorage: {
      combobox: {
        background: "#ffffff",
        dropdownBorderColor: "#d0d5da",
        hoverDropdownBorderColor: "#a3a9ae",
        isOpenDropdownBorderColor: "#2DA7DB",
        arrowFill: "#a3a9ae",
      },
      folderInput: {
        background: "#ffffff",
        borderColor: "#d0d5da",
        hoverBorderColor: "#a3a9ae",
        focusBorderColor: "#35abd8",
        rootLabelColor: "#a3a9ae",
        iconFill: "#657177",
      },
    },

    iconCropper: {
      gridColor: "#333333",
      deleteButton: {
        background: "#f8f9f9",
        hoverBackground: "#f3f4f4",
        borderColor: "#f8f9f9",
        hoverBorderColor: "#f3f4f4",
        color: "#555f65",
        iconColor: "#657077",
      },
    },

    previewTile: {
      background: "#ffffff",
      borderColor: "#d0d5da",
      iconBorderColor: "#eceef1",
    },

    dropzone: {
      borderColor: "#eceef1",
      linkMainColor: "#316daa",
      linkSecondaryColor: "#333333",
      exstsColor: "#a3a9ae",
    },
  },

  filesThirdPartyDialog: {
    border: "1px solid #d1d1d1",
  },

  connectedClouds: {
    color: "#657077",
    borderBottom: `1px solid #eceef1`,
    borderRight: `1px solid #d0d5da`,
  },

  filesModalDialog: {
    border: `1px solid lightgray`,
  },

  filesDragTooltip: {
    background: white,
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    color: gray,
  },

  filesEmptyContainer: {
    linkColor: cyanBlueDarkShade,
    privateRoom: {
      linkColor: "#116d9d",
    },
  },

  emptyContent: {
    header: {
      color: "#333333",
    },

    description: {
      color: cyanBlueDarkShade,
    },
    button: {
      colorLink: "#657077",
      colorText: "#555F65",
    },
  },

  filesPanels: {
    color: black,

    aside: {
      backgroundColor: white,
    },

    addGroups: {
      iconColor: gray,
      arrowColor: darkBlack,
    },

    addUsers: {
      iconColor: gray,
      arrowColor: darkBlack,
    },

    changeOwner: {
      iconColor: gray,
      arrowColor: darkBlack,
    },

    embedding: {
      textAreaColor: "#AEAEAE",
      iconColor: black,
      color: gray,
    },

    versionHistory: {
      borderTop: `1px solid ${grayLightMid}`,
    },

    content: {
      backgroundColor: white,
      fill: gray,
      disabledFill: grayMid,
    },

    body: {
      backgroundColor: grayLightMid,
      fill: black,
    },

    footer: {
      backgroundColor: white,
      borderTop: `1px solid ${grayLightMid}`,
    },

    linkRow: {
      backgroundColor: grayLight,
      fill: gray,
      disabledFill: grayMid,
    },

    selectFolder: {
      color: gray,
    },

    selectFile: {
      color: gray,
      background: grayLight,
      borderBottom: `1px solid ${grayLightMid}`,
      borderRight: `1px solid ${globalColors.lightGrayishBlue}`,

      buttonsBackground: white,
    },

    filesList: {
      color: gray,
      backgroundColor: grayLightMid,
      borderBottom: `1px solid ${grayLightMid}`,
    },

    modalRow: {
      backgroundColor: grayLightMid,
      fill: gray,
      disabledFill: grayMid,
    },

    sharing: {
      color: gray,
      fill: gray,
      loadingFill: grayMid,

      borderBottom: "1px solid #eceef1",
      borderTop: "1px solid #eceef1",
      externalLinkBackground: "#f8f9f9",
      externalLinkSvg: "#333333",

      internalLinkBorder: "1px dashed #333333",

      itemBorder: "1px dashed #333333",

      itemOwnerColor: "rgb(163, 169, 174)",

      backgroundButtons: "#FFFFFF",

      dropdownColor: black,

      loader: {
        foregroundColor: grayLight,
        backgroundColor: grayLight,
      },
    },

    upload: {
      color: gray,
      tooltipColor: lightCumulus,

      shareButton: {
        color: gray,
        sharedColor: grayMain,
      },

      loadingButton: {
        color: blueMain,
        background: white,
      },
    },
  },

  menuItem: {
    iconWrapper: {
      width: "16px",
      height: "16px",
      header: {
        width: "24px",
        height: "24px",
      },
    },
    separator: {
      borderBottom: `1px solid ${grayLightMid} !important`,
      margin: "6px 16px 6px 16px !important",
      height: "1px !important",
      width: "calc(100% - 32px) !important",
    },
    text: {
      header: {
        fontSize: "15px",
        lineHeight: "20px",
      },
      mobile: {
        fontSize: "13px",
        lineHeight: "36px",
      },
      fontSize: "12px",
      lineHeight: "30px",
      fontWeight: "600",
      margin: "0 0 0 8px",
      color: black,
    },
    hover: grayLight,
    background: "none",
    svgFill: black,
    header: {
      height: "49px",
      borderBottom: `1px solid ${grayLightMid}`,
      marginBottom: "6px",
    },
    height: "30px",
    borderBottom: "none",
    marginBottom: "0",
    padding: "0 12px",
    mobile: {
      height: "36px",
      padding: "0 16px 6px",
    },
  },
  newContextMenu: {
    background: white,
    borderRadius: "6px",
    mobileBorderRadius: "6px 6px 0 0",
    boxShadow: "0px 12px 40px rgba(4, 15, 27, 0.12)",
    padding: "6px 0px",
    border: "none",
    devices: {
      maxHeight: "calc(100vh - 64px)",
      tabletWidth: "375px",
      mobileWidth: "100vw",
      left: 0,
      right: 0,
      bottom: 0,
      margin: "0 auto",
    },
  },
  filesSettings: {
    color: cyanBlueDarkShade,

    linkColor: black,
  },

  filesBadges: {
    iconColor: gray,
    hoverIconColor: "#3B72A7",

    color: white,
    backgroundColor: white,

    badgeColor: white,
    badgeBackgroundColor: gray,
  },

  filesEditingWrapper: {
    color: black,
    border: `1px solid ${grayMid}`,
    borderBottom: `1px solid ${grayLightMid}`,

    tile: {
      background: globalColors.lightHover,
      itemBackground: white,
      itemBorder: grayMid,
      itemActiveBorder: blueMain,
    },

    row: {
      itemBackground: white,
    },

    fill: gray,
    hoverFill: grayMain,
  },

  filesIcons: {
    fill: "#3b72a7",
    hoverFill: "#3b72a7",
  },

  filesQuickButtons: {
    color: gray,
    sharedColor: "#3b72a7",
    hoverColor: "#3b72a7",
  },

  filesSharedButton: {
    color: gray,
    sharedColor: grayMain,
  },

  filesPrivateRoom: {
    borderBottom: "1px solid #d3d3d3",
    linkColor: "#116d9d",
    textColor: "#83888D",
  },

  filesVersionHistory: {
    row: {
      color: gray,
      fill: black,
    },

    badge: {
      color: white,
      stroke: gray,
      fill: gray,
      defaultFill: white,
      badgeFill: orangeMain,
    },

    versionList: {
      fill: grayMid,
      stroke: grayMid,
      color: grayMid,
    },
  },

  login: {
    linkColor: link,
    textColor: gray,
    navBackground: "#F8F9F9",
    headerColor: black,
    helpButton: "#A3A9AE",
    orLineColor: "#ECEEF1",
    orTextColor: "#A3A9AE",
    titleColor: black,

    register: {
      backgroundColor: grayLight,
      textColor: link,
    },

    container: {
      backgroundColor: grayLightMid,
    },
  },

  facebookButton: {
    background: white,
    border: "1px solid #1877f2",
    color: "#1877f2",
  },

  peopleSelector: {
    textColor: gray,
  },

  peopleWithContent: {
    color: gray,
    pendingColor: grayMid,
  },

  peopleDialogs: {
    modal: {
      border: `1px solid ${gray}`,
    },

    deleteUser: {
      textColor: red,
    },

    deleteSelf: {
      linkColor: link,
    },

    changePassword: {
      linkColor: link,
    },
  },

  downloadDialog: {
    background: "#f8f9f9",
  },

  client: {
    about: {
      linkColor: blueMain,
      border: "1px solid lightgray",
      logoColor: black,
    },

    comingSoon: {
      linkColor: cyanBlueDarkShade,
      linkIconColor: black,
      backgroundColor: white,
      foregroundColor: white,
    },

    confirm: {
      activateUser: {
        textColor: "#116d9d",
        textColorError: red,
      },
      change: {
        titleColor: "#116d9d",
      },
    },

    home: {
      logoColor: black,
      textColorError: red,
    },

    payments: {
      linkColor: link,
      delayColor: "#F21C0E",
    },

    paymentsEnterprise: {
      background: grayLight,

      buttonBackground: "#edf2f7",

      linkColor: link,
      headerColor: orangePressed,
    },

    settings: {
      iconFill: black,
      trashIcon: "#A3A9AE",
      article: {
        titleColor: grayMain,
        fillIcon: "dimgray",
        expanderColor: "dimgray",
      },

      separatorBorder: `1px solid ${grayLightMid}`,

      security: {
        arrowFill: black,
        descriptionColor: cyanBlueDarkShade,

        admins: {
          backgroundColor: black,
          backgroundColorWrapper: blueMain,
          roleColor: grayMid,

          color: link,
          departmentColor: gray,

          tooltipColor: lightCumulus,

          nameColor: black,
          pendingNameColor: gray,

          textColor: white,
          iconColor: blueMain,
        },

        owner: {
          backgroundColor: grayLight,
          linkColor: link,
          departmentColor: gray,
          tooltipColor: lightCumulus,
        },
        auditTrail: {
          downloadReportDescriptionColor: gray,
        },
      },

      common: {
        linkColor: gray,
        linkColorHelp: link,
        tooltipLinkColor: black,
        arrowColor: black,
        descriptionColor: grayMain,
        brandingDescriptionColor: "#657077",

        whiteLabel: {
          borderImg: "1px solid #d1d1d1",

          backgroundColorWhite: white,
          backgroundColorLight: "#F8F9F9",
          backgroundColorDark: "#282828",
          greenBackgroundColor: "#40865C",
          blueBackgroundColor: "#446995",
          orangeBackgroundColor: "#AA5252",

          dataFontColor: white,
          dataFontColorBlack: black,
        },
      },

      integration: {
        separatorBorder: `1px solid ${grayMid}`,
        linkColor: link,

        sso: {
          toggleContentBackground: grayLight,
          iconButton: black,
          iconButtonDisabled: gray,
        },

        smtp: {
          requirementColor: "#F21C0E",
        },
      },

      backup: {
        rectangleBackgroundColor: "#f8f9f9",
        separatorBorder: "1px solid #eceef1",
        warningColor: "#f21c0e",
        textColor: "#A3A9AE",
        backupCheckedListItemBackground: "#F3F4F4",
      },

      payment: {
        priceColor: "#555F65",
        storageSizeTitle: "#A3A9AE",

        backgroundColor: "#f8f9f9",
        linkColor: "#316DAA",
        tariffText: "#555F65",
        border: "1px solid #f8f9f9",
        backgroundBenefitsColor: "#f8f9f9",
        rectangleColor: "#f3f4f4",

        priceContainer: {
          backgroundText: "#f3f4f4",
          background: "transparent",
          border: "1px solid #d0d5da",
          featureTextColor: "#A3A9AE",

          disableColor: "#A3A9AE",
          trackNumberColor: "#A3A9AE",
          disablePriceColor: "#A3A9AE",
        },

        benefitsContainer: {
          iconsColor: "#657077",
        },

        contactContainer: {
          textColor: "#A3A9AE",
          linkColor: "#657077",
        },

        warningColor: "#F21C0E",
        color: "#F97A0B",
      },
    },

    wizard: {
      linkColor: "#116d9d",
      generatePasswordColor: "#657077",
    },
  },

  campaignsBanner: {
    border: "1px solid #d1d1d1",
    color: darkBlack,

    btnColor: white,
    btnBackgroundActive: blueMain,
  },

  tileLoader: {
    border: `1px solid ${grayMid}`,

    background: white,
  },

  errorContainer: {
    background: white,
    bodyText: "#A3A9AE",
  },

  editor: {
    color: "#555f65",
    background: white,
  },

  submenu: {
    lineColor: "#eceef1",
    backgroundColor: white,
    textColor: "#657077",
    activeTextColor: "#316DAA",
    bottomLineColor: "#316DAA",
  },

  hotkeys: {
    key: {
      color: grayMain,
    },
  },

  tag: {
    color: black,
    background: "#f3f4f4",
    hoverBackground: "#eceef1",
    disabledBackground: "#f8f9f9",
    defaultTagColor: black,
    newTagBackground: "#eceef1",
  },

  profile: {
    main: {
      background: "#F8F9F9",
      textColor: black,

      descriptionTextColor: "#A3A9AE",
      pendingEmailTextColor: "#A3A9AE",

      mobileRowBackground: "#F8F9F9",
    },
    themePreview: {
      descriptionColor: "#A3A9AE",
      border: "1px solid #eceef1",
    },
    notifications: {
      textDescriptionColor: "#A3A9AE",
    },
  },

  activeSessions: {
    color: "#333",
    borderColor: "#eceef1",
    tickIconColor: "#35AD17",
    removeIconColor: "#A3A9AE",
    sortHeaderColor: "#d0d5da",
  },

  formWrapper: {
    background: white,
    boxShadow: "0px 5px 20px rgba(4, 15, 27, 0.07)",
  },

  preparationPortalProgress: {
    backgroundColor: "#F3F4F4",
    colorPercentSmall: "#333333",
    colorPercentBig: "#FFFFFF",
    errorTextColor: "#F21C0E",
    descriptionTextColor: "#A3A9AE",
  },

  codeInput: {
    background: white,
    border: "1px solid #d0d5da",
    color: black,
    lineColor: "#C4C4C4",
    disabledBackground: "#F8F9F9",
    disabledBorder: "1px solid #ECEEF1",
    disabledColor: "#A3A9AE",
  },

  accessRightSelect: {
    descriptionColor: gray,
  },

  itemIcon: {
    borderColor: grayLightMid,
  },

  invitePage: {
    borderColor: "#eceef1",
  },

  portalUnavailable: {
    textDescriptionColor: "#A3A9AE",
  },

  emailChips: {
    borderColor: "#A3A9AE",
    dashedBorder: "1px dashed #5299E0",
  },

  editLink: {
    text: {
      color: "#A3A9AE",
      errorColor: "#F21C0E",
    },
  },
};

export default Base;
