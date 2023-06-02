import React from "react";
import ReactDOM from "react-dom";
import PropType from "prop-types";
import PropTypes from "prop-types";
import Countdown, { zeroPad } from "react-countdown";
import { StyledAction, StyledSnackBar, StyledIframe } from "./styled-snackbar";
import StyledCrossIcon from "./styled-snackbar-action";
import StyledLogoIcon from "./styled-snackbar-logo";
import Box from "../box";
import Heading from "../heading";
import Text from "../text";

class SnackBar extends React.Component {
  constructor(props) {
    super(props);
    this.state = { isLoaded: false };
  }
  static show(barConfig) {
    const { parentElementId, ...rest } = barConfig;

    let parentElementNode =
      parentElementId && document.getElementById(parentElementId);

    if (!parentElementNode) {
      const snackbarNode = document.createElement("div");
      snackbarNode.id = "snackbar";
      document.body.appendChild(snackbarNode);
      parentElementNode = snackbarNode;
    }

    window.snackbar = barConfig;

    ReactDOM.render(<SnackBar {...rest} />, parentElementNode);
  }

  static close() {
    if (window.snackbar && window.snackbar.parentElementId) {
      const snackbar = document.querySelector("#snackbar-container");
      snackbar.remove();
      //ReactDOM.unmountComponentAtNode(window.snackbar.parentElementId);
    }
  }

  onActionClick = (e) => {
    this.props.onAction && this.props.onAction(e);
  };

  componentDidMount() {
    const { onLoad } = this.props;
    onLoad();
  }

  bannerRenderer = () => {
    const { htmlContent, sectionWidth } = this.props;
    return (
      <div id="bar-banner" style={{ position: "relative" }}>
        <StyledIframe
          id="bar-frame"
          src={htmlContent}
          scrolling="no"
          sectionWidth={sectionWidth}
          onLoad={() => {
            this.setState({ isLoaded: true });
          }}
        ></StyledIframe>
        {this.state.isLoaded && (
          <StyledAction className="action" onClick={this.onActionClick}>
            <StyledCrossIcon size="medium" />
          </StyledAction>
        )}
      </div>
    );
  };

  // Renderer callback with condition
  countDownRenderer = ({ minutes, seconds, completed }) => {
    if (completed) return <></>;
    const { textColor, fontSize, fontWeight } = this.props;

    // Render a countdown
    return (
      <Text
        as="p"
        color={textColor}
        fontSize={fontSize}
        fontWeight={fontWeight}
      >
        {zeroPad(minutes)}:{zeroPad(seconds)}
      </Text>
    );
  };

  render() {
    const {
      text,
      headerText,
      btnText,
      textColor,
      showIcon,
      fontSize,
      fontWeight,
      textAlign,
      htmlContent,
      style,
      countDownTime,
      isCampaigns,
      onAction,
      ...rest
    } = this.props;

    const headerStyles = headerText ? {} : { display: "none" };

    const bannerElement = this.bannerRenderer();

    return (
      <>
        {isCampaigns ? (
          <>{bannerElement}</>
        ) : (
          <StyledSnackBar id="snackbar-container" style={style} {...rest}>
            {htmlContent ? (
              <div
                dangerouslySetInnerHTML={{
                  __html: htmlContent,
                }}
              />
            ) : (
              <div className="text-container">
                <div className="header-body" textalign={textAlign}>
                  {showIcon && (
                    <Box className="logo">
                      <StyledLogoIcon size="medium" color={textColor} />
                    </Box>
                  )}

                  <Heading
                    size="xsmall"
                    isInline={true}
                    className="text-header"
                    style={headerStyles}
                    color={textColor}
                  >
                    {headerText}
                  </Heading>
                </div>
                <div className="text-body">
                  <Text
                    as="p"
                    className={"text"}
                    color={textColor}
                    fontSize={fontSize}
                    fontWeight={fontWeight}
                    noSelect
                  >
                    {text}
                  </Text>

                  {btnText && (
                    <Text
                      color={textColor}
                      className="button"
                      onClick={this.onActionClick}
                    >
                      {btnText}
                    </Text>
                  )}

                  {countDownTime > -1 && (
                    <Countdown
                      date={Date.now() + countDownTime}
                      renderer={this.countDownRenderer}
                      onComplete={this.onActionClick}
                    />
                  )}
                </div>
              </div>
            )}
            {!btnText && (
              <button className="action" onClick={this.onActionClick}>
                <StyledCrossIcon size="small" />
              </button>
            )}
          </StyledSnackBar>
        )}
      </>
    );
  }
}

SnackBar.propTypes = {
  /** Specifies the Snackbar text */
  text: PropTypes.oneOfType([PropTypes.object, PropTypes.string]),
  /** Specifies the header text */
  headerText: PropTypes.string,
  /** Specifies the button text */
  btnText: PropTypes.string,
  /** Specifies the source of the image used as the Snackbar background  */
  backgroundImg: PropTypes.string,
  /** Specifies the background color */
  backgroundColor: PropTypes.string,
  /** Specifies the text color */
  textColor: PropTypes.string,
  /** Displays the icon */
  showIcon: PropTypes.bool,
  /** Sets a callback function that is triggered when the Snackbar is clicked */
  onAction: PropTypes.func,
  /** Sets the font size  */
  fontSize: PropTypes.string,
  /** Sets the font weight */
  fontWeight: PropTypes.string,
  /** Specifies the text alignment */
  textAlign: PropTypes.string,
  /** Allows displaying content in HTML format */
  htmlContent: PropTypes.string,
  /** Accepts css */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets the countdown time */
  countDownTime: PropType.number,
  /** Sets the section width */
  sectionWidth: PropTypes.number,
  /** Required in case the snackbar is a campaign banner */
  isCampaigns: PropTypes.bool,
  /** Used as an indicator that a web page has fully loaded, including its content, images, style files, and external scripts */
  onLoad: PropTypes.func,
  /** Required in case the snackbar is a notification banner */
  isMaintenance: PropTypes.bool,
  /** Sets opacity */
  opacity: PropTypes.number,
};

SnackBar.defaultProps = {
  backgroundColor: "#F7E6BE",
  textColor: "#000",
  showIcon: true,
  fontSize: "13px",
  fontWeight: "400",
  textAlign: "left",
  htmlContent: "",
  countDownTime: -1,
  isCampaigns: false,
};

export default SnackBar;
