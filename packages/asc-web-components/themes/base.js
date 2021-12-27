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
  strongBlue,
  lightGrayishStrongBlue,
  darkRed,
} = globalColors;

const Base = {
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
    color: "#333333",
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
      base: "24px",
      medium: "32px",
      big: "36px",
      large: "44px",
    },

    lineHeight: {
      base: "15px",
      medium: "18px",
      big: "20px",
      large: "20px",
    },

    fontSize: {
      base: "12px",
      medium: "13px",
      big: "14px",
      large: "16px",
    },

    padding: {
      base: "0 14px",
      medium: "0 18px",
      big: "0 20px",
    },

    minWidth: {
      base: "65px",
      medium: "80px",
      big: "85px",
    },

    color: {
      base: black,
      baseHover: black,
      baseActive: black,
      baseDisabled: grayLightMid,
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
      primary: blueMain,
      primaryHover: blueHover,
      primaryActive: blueActive,
      primaryDisabled: blueDisabled,
    },

    border: {
      base: `1px solid ${globalColors.grayMid}`,
      baseHover: `1px solid ${globalColors.blueMain}`,
      baseActive: `1px solid ${globalColors.blueMain}`,
      baseDisabled: `1px solid ${globalColors.grayLightMid}`,
      primary: `1px solid ${globalColors.blueMain}`,
      primaryHover: `1px solid ${globalColors.blueHover}`,
      primaryActive: `1px solid ${globalColors.blueActive}`,
      primaryDisabled: `1px solid ${globalColors.blueDisabled}`,
    },

    loader: {
      base: black,
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

  mainButton: {
    backgroundColor: orangeMain,
    disableBackgroundColor: orangeDisabled,
    hoverBackgroundColor: orangeHover,
    clickBackgroundColor: orangePressed,

    padding: "5px 10px",
    borderRadius: "3px",
    lineHeight: "22px",
    fontSize: "15px",
    fontWeight: 700,
    textColor: "#FFF",

    cornerRoundsTopRight: "0",
    cornerRoundsBottomRight: "0",

    svg: {
      margin: "auto",
      height: "100%",
      fill: "#ffffff",
    },

    secondaryButton: {
      height: "32px",
      padding: "0",
      borderRadius: "3px",
      cornerRoundsTopLeft: "0",
      cornerRoundsBottomLeft: "0",
    },

    dropDown: {
      width: "100%",
      top: "100%",
    },

    arrowDropdown: {
      borderLeft: "4px solid transparent",
      borderRight: "4px solid transparent",
      borderTop: "4px solid white",
      right: "10px",
      top: "50%",
      width: "0",
      height: "0",
      marginTop: " -1px",
    },
  },

  socialButton: {
    fontWeight: "600",
    textDecoration: "none",
    padding: "0",
    borderRadius: "2px",
    height: "40px",
    textAlign: "left",
    stroke: " none",
    outline: "none",
    width: "100%",

    background: white,
    disableBackgroundColor: "rgba(0, 0, 0, 0.08)",
    hoverBackground: white,
    activeBackground: grayMaxLight,

    boxShadow:
      "0px 1px 1px rgba(0, 0, 0, 0.24),0px 0px 1px rgba(0, 0, 0, 0.12)",
    hoverBoxShadow:
      "0px 2px 2px rgba(0, 0, 0, 0.24),0px 0px 2px rgba(0, 0, 0, 0.12)",

    color: "rgba(0, 0, 0, 0.54)",
    disableColor: "rgba(0, 0, 0, 0.4)",
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
      color: "#757575",
    },

    svg: {
      margin: "11px",
      width: "18px",
      height: "18px",
      minWidth: "18px",
      minHeight: "18px",
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
    color: "#A3A9AE",
    hoverColor: "#657077",
  },
  selectorAddButton: {
    background: grayLight,
    activeBackground: grayLightMid,

    border: `1px solid ${globalColors.grayLightMid}`,
    boxSizing: "border-box",
    borderRadius: "3px",
    height: " 34px",
    width: "34px",
    padding: "9px",
    color: "#979797",
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
      color: "#555F65",
      disabledColor: "#D0D5DA",
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

    marginRight: "4px",

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
    minWidth: "160px",
    overflow: "hidden",
    textOverflow: "ellipsis",

    element: {
      marginRight: "14px",
      marginLeft: "2px",
    },

    optionButton: {
      padding: "8px 0px 9px 7px",
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
    color: "#FFFFFF",
    backgroundColor: "#ED7309",
  },

  scrollbar: {
    backgroundColorVertical: "rgba(208, 213, 218, 1)",
    backgroundColorHorizontal: "rgba(0, 0, 0, 0.1)",
    hoverBackgroundColorVertical: "rgba(163, 169, 174, 1)",
  },

  modalDialog: {
    width: "auto",
    maxwidth: "560px",
    margin: " 0 auto",
    minHeight: "100%",

    content: {
      backgroundColor: white,
      padding: "0 16px 16px",

      heading: {
        maxWidth: "500px",
        margin: "0",
        lineHeight: "56px",
        fontWeight: "700",
      },
    },

    header: {
      borderBottom: `1px solid ${globalColors.lightGrayishBlue}`,
    },

    closeButton: {
      width: "17px",
      height: "17px",
      minWidth: "17px",
      minHeight: "17px",

      right: "16px",
      top: "19px",
      hoverColor: grayMain,
    },
  },

  paging: {
    button: {
      marginRight: "8px",
      maxWidth: "110px",
      padding: "6px 8px 10px",
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
    errorBorderColor: red,
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

    paddingRight: {
      base: "37px",
      middle: "48px",
      big: "53px",
      huge: "58px",
      large: "64px",
    },

    icon: {
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
        large: "43px",
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

    iconColor: "#D0D5DA",
    hoverIconColor: "#D0D5DA",
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
      base: "14px",
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
      large: "11px 15px",
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

    iconColor: "#D0D5DA",
    hoverIconColor: "#D0D5DA",
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
    focusErrorBorderColor: red,
    focusOutline: "none",

    scrollWidth: "100%",
    scrollHeight: "91px",
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
    textDecoration: "underline dashed",
    disableColor: gray,

    svg: {
      opacity: "1",
      semiTransparentOpacity: "0.5",
    },

    text: { maxWidth: "100%" },

    span: { maxWidth: "300px" },

    caret: {
      width: "8px",
      minWidth: "8px",
      height: "8px",
      minHeight: "8px",
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
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    opacity: "1",
    padding: "16px",
    pointerEvents: "auto",
    maxWidth: "340px",
    color: "#f8f7bf",

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
      height: "50px",
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
        hoverColor: white,
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
        lineHeight: "13px",
        height: "15px",
      },

      labelIcon: {
        width: "100%",
        margin: "0 0 8px 0",
      },

      body: {
        width: "100%",
      },

      iconButton: {
        margin: "0",
        padding: "0px 8px",
        width: "13px",
        height: "13px",
      },
    },
  },

  avatar: {
    initialsContainer: {
      color: white,
      left: "50%",
      top: "50%",
      transform: "translate(-50%, -50%)",
      fontWeight: "600",

      fontSize: {
        min: "12px",
        small: "12px",
        medium: "20px",
        big: "34px",
        max: "72px",
      },
    },

    roleWrapperContainer: {
      left: {
        min: "-2px",
        small: "-2px",
        medium: "-4px",
        big: "0px",
        max: "0px",
      },

      bottom: {
        min: "3px",
        small: "3px",
        medium: "6px",
        big: "5px",
        max: "0px",
      },

      width: {
        medium: "14px",
        max: "24px",
      },

      height: {
        medium: "14px",
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
      fill: "#ED7309",
      stroke: "#000000",
      color: "#ffffff",
    },

    guest: {
      fill: "#3B72A7",
      stroke: "#000000",
      color: "#ffffff",
    },

    owner: {
      fill: "#EDC409",
      stroke: "#000000",
      color: "#ffffff",
    },

    editContainer: {
      right: "0px",
      bottom: "0px",
      fill: "#ffffff",
      backgroundColor: "#265a8f",
      borderRadius: "50%",
      height: "32px",
      width: "32px",
    },

    image: {
      width: "100%",
      height: "auto",
      borderRadius: "50%",
    },

    width: {
      min: "32px",
      small: "36px",
      medium: "48px",
      big: "82px",
      max: "160px",
    },

    height: {
      min: "32px",
      small: "36px",
      medium: "48px",
      big: "82px",
      max: "160px",
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
      linkColor: "#316DAA",
    },

    slider: {
      width: "100%",
      margin: "8px 0",
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
        border: `6px solid ${white}`,
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
    backgroundColor: "rgba(6, 22, 38, 0.1)",
    unsetBackgroundColor: "unset",
  },

  treeMenu: {
    disabledColor: "#767676",
  },

  treeNode: {
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
    zIndex: "200",
    background: white,
    borderRadius: "6px",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
  },

  dropDownItem: {
    color: black,
    disableColor: gray,
    backgroundColor: white,
    hoverBackgroundColor: grayLight,
    hoverDisabledBackgroundColor: white,
    fontWeight: "600",
    fontSize: "13px",
    width: "100%",
    maxWidth: "500px",
    border: "0px",
    margin: "0px",
    padding: "0px 16px",
    lineHeight: "32px",
    tabletLineHeight: "36px",

    icon: {
      width: "16px",
      marginRight: "8px",
      lineHeight: "14px",

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

    closeButtonColor: "#333333",
  },

  loader: {
    color: shuttleGrey,
    size: "40px",
    marginRight: "2px",
    borderRadius: "50%",
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

    width: {
      base: "173px",
      middle: "300px",
      big: "350px",
      huge: "500px",
    },

    arrow: {
      width: "8px",
      flex: "0 0 8px",
      marginTopWithBorder: "5px",
      marginTop: "12px",
      marginRight: "8px",
      marginLeft: "auto",
      fillColor: gray,
    },

    button: {
      height: "18px",
      heightWithBorder: "30px",
      paddingLeft: "8px",

      color: black,
      disabledColor: grayMid,
      background: white,
      backgroundWithBorder: "none",

      border: `1px solid ${grayMid}`,
      borderRadius: "3px",
      borderColor: blueMain,
      openBorderColor: blueMain,
      disabledBorderColor: grayLightMid,
      disabledBackground: grayLight,

      hoverBorderColor: gray,
      hoverBorderColorOpen: blueMain,
      hoverDisabledBorderColor: grayLightMid,
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
    arrowMarginRight: "9px",
    arrowMarginBottom: "5px",
    transform: "rotate(180deg)",
    iconColor: black,

    childrenContent: {
      color: black,
      paddingTop: "6px",
    },
  },

  toggleButton: {
    fillColor: blueMain,
    fillColorOff: gray,

    disableFillColor: grayLightMid,
    disableFillColorOff: grayLightMid,

    borderColor: blueMain,
    borderColorOff: gray,

    disableBorderColor: grayLightMid,
    disableBorderColorOff: grayLightMid,

    fillCircleColor: white,
    fillCircleColorOff: white,

    disableFillCircleColor: white,
    disableFillCircleColorOff: white,
  },

  contextMenuButton: {
    content: {
      width: "100%",
      backgroundColor: " #fff",
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
    baseWidth: "265px",
    bigWidth: "289px",

    baseMaxWidth: "293px",
    bigMaxWidth: "325px",

    hover: {
      backgroundColor: grayLightMid,
      borderRadius: "16px",
      cursor: "pointer",
    },

    day: {
      width: "32px",
      height: "32px",
      baseSizeWidth: "270px",
      bigSizeWidth: "294px",
      baseMarginTop: "3px",
      bigMarginTop: "7.5px",
      lineHeight: "33px",
    },

    weekdays: {
      color: black,
      disabledColor: gray,
      baseWidth: "272px",
      bigWidth: "295px",
      marginBottom: "-5px",
    },

    month: {
      baseWidth: "267px",
      bigWidth: "295px",
      color: black,
      weekendColor: gray,
      disabledColor: grayLightMid,
      neighboringHoverColor: black,
      neighboringColor: grayLightMid,
    },

    selectedDay: {
      backgroundColor: orangeMain,
      borderRadius: "16px",
      cursor: "pointer",
      color: white,
    },

    comboBox: {
      color: black,
      minWidth: "80px",
      height: "32px",
      marginLeft: "8px",
      padding: "0 0 24px 0",
    },
    comboBoxMonth: {
      baseWidth: "172px",
      bigWidth: "205px",

      baseMaxWidth: "172px",
      bigMaxWidth: "196px",
    },
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

  nav: {
    backgroundColor: "#0F4071",
  },

  navItem: {
    baseColor: "#7A95B0",
    activeColor: "#FFFFFF",
    separatorColor: "#3E668D",

    wrapper: {
      hoverBackground: "#0d3760",
    },
  },

  header: {
    backgroundColor: "#0F4071",
    linkColor: "#7a95b0",
  },

  menuContainer: {
    background: "linear-gradient(200.71deg, #2274aa 0%, #0f4071 100%)",
    color: "#FFFFFF",
  },

  article: {
    background: "#f8f9f9",
    pinBorderColor: "#eceef1",
  },

  section: {
    toggler: {
      background: "#fff",
      boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    },

    header: {
      backgroundColor: "#fff",
    },
  },

  filesArticleBody: {
    background: "#dfe2e3",
    fill: "#657077",
    expanderColor: "dimgray",
    downloadAppList: {
      color: "#83888d",
    },
    thirdPartyList: {
      color: "#818b91",
      linkColor: "#555f65",
    },
  },

  peopleArticleBody: {
    iconColor: "#657077",
    expanderColor: "dimgray",
  },

  peopleTableRow: {
    nameColor: "#333333",
    pendingNameColor: "#A3A9AE",

    sideInfoColor: "#A3A9AE",
    pendingSideInfoColor: "#D0D5DA",
  },

  filterInput: {
    filterButton: {
      stroke: "#a3a9ae",
      fill: "#eceef1",
    },

    comboButtonLabelColor: "#333333",
    comboButtonLabelColorTwo: "#a3a9ae",

    viewSelector: {
      border: "#D0D5DA",
      disabledBorder: "#ECEEF1",

      disabledBackground: "#F8F9F9",

      activeBackground: "#a3a9ae",
      activeBorder: "#a3a9ae",
    },

    filterItem: {
      border: "1px solid #eceef1",
      backgroundColor: "#f8f9f9",
      color: "#555f65",
    },

    content: {
      color: "#333333",
      background: "#eceef1",
    },

    closeButton: {
      borderLeft: "1px solid #eceef1",
      background: "#f8f9f9",

      activeBackground: "#eceef1",
      activeFill: "#a3a9ae",

      hoverFill: "#555f65",
    },

    hideButton: {
      border: "1px solid #eceef1",
      background: "#f8f9f9",

      hoverBorder: "#A3A9AE",
      disabledHoverBorder: "#ECEEF1",

      activeBackground: "#ECEEF1",
      disabledActiveBackground: "#F8F9F9",
    },
  },

  profileInfo: {
    color: "#83888d",
    iconButtonColor: "#333333",
    linkColor: "#A3A9AE",
  },

  tableContainer: {
    borderRight: "2px solid #d0d5da",
    hoverBorderColor: "#657077",
    tableCellBorder: "1px solid #eceef1",

    groupMenu: {
      background: "#fff",
      borderRight: "1px solid #d0d5da",
    },

    header: {
      background: "#fff",
      borderBottom: "1px solid #eceef1",
      textColor: gray,
      activeTextColor: grayMain,
      hoverTextColor: "#eeeeee",

      iconColor: gray,
      activeIconColor: grayMain,
      hoverIconColor: "#657077",

      borderImageSource:
        "linear-gradient(to right,#ffffff 24px,#eceef1 24px,#eceef1 calc(100% - 24px),#ffffff calc(100% - 24px))",
    },

    tableCell: {
      border: "1px solid #eceef1",
    },
  },

  filesSection: {
    rowView: {
      checkedBackground: "#f3f4f4",

      draggingBackground: "#f8f7bf",
      draggingHoverBackground: "#efefb2",

      shareButton: {
        color: "#657077",
        fill: "#657077",
      },

      sideColor: "#A3A9AE",
      linkColor: "#333333",
      textColor: "#A3A9AE",
    },

    tableView: {
      fileName: {
        linkColor: "#333",
        textColor: "#A3A9AE",
      },

      row: {
        checkboxChecked:
          "linear-gradient(to right, #f3f4f4 24px, #eceef1 24px)",
        checkboxDragging:
          "linear-gradient(to right, #f8f7bf 24px, #eceef1 24px)",
        checkboxDraggingHover:
          "inear-gradient(to right,rgb(239, 239, 178) 24px, #eceef1 24px)",

        contextMenuWrapperChecked:
          "linear-gradient(to left, #f3f4f4 24px, #eceef1 24px)",
        contextMenuWrapperDragging:
          "border-image-source: linear-gradient(to left, #f8f7bf 24px, #eceef1 24px)",
        contextMenuWrapperDraggingHover:
          "linear-gradient(to left,rgb(239, 239, 178) 24px,#eceef1 24px)",

        backgroundActive: "#F3F4F4",

        borderImageCheckbox:
          "linear-gradient(to right, #ffffff 24px, #eceef1 24px)",
        borderImageContextMenu:
          "linear-gradient(to left, #ffffff 24px, #eceef1 24px)",

        borderHover: "#a3a9ae",
        sideColor: gray,
      },
    },

    tilesView: {
      tile: {
        draggingColor: "#f8f7bf",
        draggingHoverColor: "#efefb2",
        checkedColor: "#f3f4f4",
        border: "1px solid #d0d5da",
        backgroundColor: "#fff",

        backgroundColorTop: "#f8f9f9",
      },

      sideColor: "#333",
      color: "#333",
      textColor: "#A3A9AE",
    },
  },

  advancedSelector: {
    footerBorder: "1px solid #eceef1",

    hoverBackgroundColor: "#eceef1",
    selectedBackgroundColor: "#eceef1",
    borderLeft: "1px solid #eceef1",

    searcher: {
      hoverBorderColor: "#d0d5da",
      focusBorderColor: "#2da7db",
      placeholderColor: "#a3a9ae",
    },
  },

  floatingButton: {
    backgroundColor: "#fff",
    color: "#2DA7DB",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    fill: "#A3A9AE",
  },

  mediaViewer: {
    color: "#d1d1d1",
    background: "rgba(17, 17, 17, 0.867)",
    backgroundColor: "rgba(11, 11, 11, 0.7)",
    fill: "#fff",
    titleColor: "#fff",
    iconColor: "#fff",

    controlBtn: {
      backgroundColor: "rgba(200, 200, 200, 0.2)",
    },

    imageViewer: {
      backgroundColor: "rgba(200, 200, 200, 0.2)",
      inactiveBackgroundColor: "rgba(11,11,11,0.7)",
      fill: "#fff",
    },

    progressBar: {
      background: "#d1d1d1",
      backgroundColor: "rgba(200, 200, 200, 0.2)",
    },

    scrollButton: {
      backgroundColor: "rgba(11, 11, 11, 0.7)",
      background: "rgba(200, 200, 200, 0.2)",
      border: "solid #fff",
    },

    videoViewer: {
      fill: "#fff",
      stroke: "#fff",
      color: "#d1d1d1",
      colorError: "#fff",
      backgroundColorError: "#000",
      backgroundColor: "rgba(11, 11, 11, 0.7)",
      background: "rgba(200, 200, 200, 0.2)",
    },
  },

  filesThirdPartyDialog: {
    border: "1px solid #d1d1d1",
  },

  filesModalDialog: {
    border: `1px solid lightgray`,
  },

  filesDragTooltip: {
    background: "#fff",
    boxShadow: "0px 5px 20px rgba(0, 0, 0, 0.13)",
    color: "#a3a9ae",
  },

  filesEmptyContainer: {
    linkColor: "#555f65",
    privateRoom: {
      linkColor: "#116d9d",
    },
  },

  filesPanels: {
    color: "#333",

    aside: {
      backgroundColor: "#fff",
    },

    addGroups: {
      iconColor: "#A3A9AE",
      arrowColor: "#000000",
    },

    addUsers: {
      iconColor: "#A3A9AE",
      arrowColor: "#000000",
    },

    changeOwner: {
      iconColor: "#A3A9AE",
      arrowColor: "#000000",
    },

    embedding: {
      textAreaColor: "#AEAEAE",
      iconColor: "#333",
      color: "#A3A9AE",
    },

    versionHistory: {
      borderTop: "1px solid #eceef1",
    },

    content: {
      backgroundColor: "#fff",
      fill: "#A3A9AE",
      disabledFill: "#D0D5DA",
    },

    body: {
      backgroundColor: "#eceef1",
      fill: "#333",
    },

    footer: {
      backgroundColor: "#fff",
      borderTop: "1px solid #eceef1",
    },

    linkRow: {
      backgroundColor: "#f8f9f9",
      fill: "#A3A9AE",
      disabledFill: "#D0D5DA",
    },

    selectFolder: {
      color: "#a3a9ae",
    },

    selectFile: {
      borderBottom: "1px solid #eceef1",
      borderRight: "1px solid #dee2e6",
    },

    filesList: {
      color: "#a3a9ae",
      backgroundColor: "#eceef1",
      borderBottom: "1px solid #eceef1",
    },

    modalRow: {
      backgroundColor: "#eceef1",
      fill: "#A3A9AE",
      disabledFill: "#D0D5DA",
    },

    sharing: {
      color: "#a3a9ae",
      fill: "#A3A9AE",
      loadingFill: "#D0D5DA",

      dropdownColor: "#333",

      loader: {
        foregroundColor: "#f8f9f9",
        backgroundColor: "#f8f9f9",
      },
    },

    upload: {
      color: "#a3a9ae",
      tooltipColor: "#f8f7bf",

      shareButton: {
        color: "#a3a9ae",
        sharedColor: "#657077",
      },

      loadingButton: {
        color: blueMain,
        background: "#fff",
      },
    },
  },
};

export default Base;
