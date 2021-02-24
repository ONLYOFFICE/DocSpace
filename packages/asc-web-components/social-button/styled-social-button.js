import styled, { css } from "styled-components";
import Base from "@appserver/components/themes/base";

const ButtonWrapper = ({ label, iconName, isDisabled, ...props }) => (
  <button type="button" {...props}></button>
);

ButtonWrapper.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func,
};

const StyledSocialButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  font-family: ${(props) => props.theme.fontFamily};
  border: none;
  display: inline-block;

  font-weight: ${(props) => props.theme.socialButton.fontWeight};
  text-decoration: ${(props) => props.theme.socialButton.textDecoration};
  margin: ${(props) => props.theme.socialButton.margin};
  padding: ${(props) => props.theme.socialButton.padding};
  border-radius: ${(props) => props.theme.socialButton.borderRadius};
  width: ${(props) => props.theme.socialButton.width};
  height: ${(props) => props.theme.socialButton.height};
  text-align: ${(props) => props.theme.socialButton.textAlign};

  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;

  stroke: ${(props) => props.theme.socialButton.stroke};

  &:focus {
    outline: ${(props) => props.theme.socialButton.outline};
  }

  ${(props) =>
    !props.isDisabled
      ? css`
          background: ${(props) => props.theme.socialButton.background};
          box-shadow: ${(props) => props.theme.socialButton.boxShadow};
          color: ${(props) => props.theme.socialButton.color};

          :hover,
          :active {
            cursor: pointer;
            box-shadow: ${(props) => props.theme.socialButton.hoverBoxShadow};
          }

          :hover {
            background: ${(props) => props.theme.socialButton.hoverBackground};
          }

          :active {
            background: ${(props) => props.theme.socialButton.activeBackground};
            border: none;
          }
        `
      : css`
          box-shadow: none;
          background: ${(props) =>
            props.theme.socialButton.disableBackgroundColor};
          color: ${(props) => props.theme.socialButton.disableColor};

          svg path {
            fill: ${(props) => props.theme.socialButton.disableColor};
          }
        `};

  .social_button_text {
    position: absolute;

    width: ${(props) => props.theme.socialButton.text.width};
    height: ${(props) => props.theme.socialButton.text.height};
    margin: ${(props) => props.theme.socialButton.text.margin};
    font-family: Roboto, "Open Sans", sans-serif, Arial;
    font-style: normal;
    font-weight: ${(props) => props.theme.socialButton.text.fontWeight};
    font-size: ${(props) => props.theme.socialButton.text.fontSize};
    line-height: ${(props) => props.theme.socialButton.text.lineHeight};
    letter-spacing: ${(props) => props.theme.socialButton.text.letterSpacing};
    user-select: none;
    overflow: ${(props) => props.theme.socialButton.text.overflow};
    text-overflow: ${(props) => props.theme.socialButton.text.textOverflow};
    white-space: ${(props) => props.theme.socialButton.text.whiteSpace};
  }

  svg {
    margin: ${(props) => props.theme.socialButton.svg.margin};
    width: ${(props) => props.theme.socialButton.svg.width};
    height: ${(props) => props.theme.socialButton.svg.height};
    min-width: ${(props) => props.theme.socialButton.svg.minWidth};
    min-height: ${(props) => props.theme.socialButton.svg.minHeight};
  }
`;

StyledSocialButton.defaultProps = { theme: Base };

export default StyledSocialButton;
