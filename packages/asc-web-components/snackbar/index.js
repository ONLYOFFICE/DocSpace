import React from "react";
import ReactDOM from "react-dom";
import PropType from "prop-types";
import PropTypes from "prop-types";
import StyledSnackBar from "./styled-snackbar";
import StyledCrossIcon from "./styled-snackbar-action";
import StyledLogoIcon from "./styled-snackbar-logo";
import Box from "../box";
import Heading from "../heading";
import Text from "../text";

class SnackBar extends React.Component {
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
      const bar = document.querySelector(`#${window.snackbar.parentElementId}`);
      bar.remove();
      //ReactDOM.unmountComponentAtNode(window.snackbar.parentElementId);
    }
  }

  onActionClick = (e) => {
    this.props.onAction && this.props.onAction(e);
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
      ...rest
    } = this.props;

    const headerStyles = headerText ? {} : { display: "none" };

    return (
      <StyledSnackBar style={style} {...rest}>
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
            <Box className="text-container" textAlign={textAlign}>
              <Heading
                size="xsmall"
                isInline={true}
                className="text-header"
                style={headerStyles}
                color={textColor}
              >
                {headerText}
              </Heading>
              <div className="text-body" textAlign={textAlign}>
                <Text
                  as="p"
                  color={textColor}
                  fontSize={fontSize}
                  fontWeight={fontWeight}
                >
                  {text}
                </Text>

                {btnText && (
                  <button className="button" onClick={this.onActionClick}>
                    <Text color={textColor}>{btnText}</Text>
                  </button>
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
        )
      </StyledSnackBar>
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
  onAction: PropType.func,
  fontSize: PropType.string,
  fontWeight: PropType.string,
  textAlign: PropType.string,
  htmlContent: PropType.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SnackBar.defaultProps = {
  backgroundColor: "#f8f7bf",
  textColor: "#000",
  showIcon: true,
  fontSize: "13px",
  fontWeight: "400",
  textAlign: "left",
  htmlContent: "",
};

export default SnackBar;
