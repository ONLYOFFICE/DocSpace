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
    this.props.clickAction && this.props.clickAction(e);
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
              <>
                {showIcon && (
                  <Box className="logo">
                    <StyledLogoIcon size="medium" color={textColor} />
                  </Box>
                )}
                <Box className="text-container" textalign={textAlign}>
                  <Heading
                    size="xsmall"
                    isInline={true}
                    className="text-header"
                    style={headerStyles}
                    color={textColor}
                  >
                    {headerText}
                  </Heading>
                  <div className="text-body" textalign={textAlign}>
                    <Text
                      as="p"
                      color={textColor}
                      fontSize={fontSize}
                      fontWeight={fontWeight}
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
                </Box>
              </>
            )}
            {!btnText && (
              <button className="action" onClick={this.onActionClick}>
                <StyledCrossIcon size="medium" />
              </button>
            )}
          </StyledSnackBar>
        )}
      </>
    );
  }
}

SnackBar.propTypes = {
  text: PropType.string,
  headerText: PropType.string,
  btnText: PropType.string,
  backgroundImg: PropType.string,
  backgroundColor: PropType.string,
  textColor: PropType.string,
  showIcon: PropType.bool,
  clickAction: PropType.func,
  fontSize: PropType.string,
  fontWeight: PropType.string,
  textAlign: PropType.string,
  htmlContent: PropType.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  countDownTime: PropType.number,
  sectionWidth: PropTypes.number,
  isCampaigns: PropTypes.bool,
  onLoad: PropTypes.func,
  isMaintenance: PropTypes.bool,
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
