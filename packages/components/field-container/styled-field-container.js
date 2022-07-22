import styled, { css } from "styled-components";
import { tablet } from "../utils/device";
import Base from "../themes/base";
function getHorizontalCss(labelWidth) {
  return css`
    display: flex;
    flex-direction: row;
    align-items: start;
    margin: ${(props) => props.theme.fieldContainer.horizontal.margin};

    .field-label {
      line-height: ${(props) =>
        props.theme.fieldContainer.horizontal.label.lineHeight};
      margin: ${(props) => props.theme.fieldContainer.horizontal.label.margin};
      position: relative;
    }
    .field-label-icon {
      display: inline-flex;
      min-width: ${labelWidth};
      width: ${labelWidth};
    }
    .field-body {
      flex-grow: ${(props) =>
        props.theme.fieldContainer.horizontal.body.flexGrow};
    }
    .icon-button {
      position: relative;
      margin-top: ${(props) =>
        props.theme.fieldContainer.horizontal.iconButton.marginTop};
      margin-left: ${(props) =>
        props.theme.fieldContainer.horizontal.iconButton.marginLeft};
    }
  `;
}

function getVerticalCss() {
  return css`
    display: flex;
    flex-direction: column;
    align-items: start;
    margin: ${(props) => props.theme.fieldContainer.vertical.margin};

    .field-label {
      line-height: ${(props) =>
        props.theme.fieldContainer.vertical.label.lineHeight};
      height: ${(props) => props.theme.fieldContainer.vertical.label.height};
      display: inline-block;
    }
    .field-label-icon {
      display: inline-flex;
      width: ${(props) => props.theme.fieldContainer.vertical.labelIcon.width};
      margin: ${(props) =>
        props.theme.fieldContainer.vertical.labelIcon.margin};
    }
    .field-body {
      width: ${(props) => props.theme.fieldContainer.vertical.body.width};
    }
    .icon-button {
      position: relative;
      margin: ${(props) =>
        props.theme.fieldContainer.vertical.iconButton.margin};
      padding: ${(props) =>
        props.theme.fieldContainer.vertical.iconButton.padding};
      width: ${(props) => props.theme.fieldContainer.vertical.iconButton.width};
      height: ${(props) =>
        props.theme.fieldContainer.vertical.iconButton.height};
    }
  `;
}

const Container = styled.div`
  .error-label {
    max-width: ${(props) => (props.maxwidth ? props.maxwidth : "293px")};
    color: ${(props) =>
      props.color ? props.color : props.theme.fieldContainer.errorLabel.color};
  }
  ${(props) =>
    props.vertical ? getVerticalCss() : getHorizontalCss(props.maxLabelWidth)}

  @media ${tablet} {
    ${getVerticalCss()}
  }
`;

Container.defaultProps = { theme: Base };
export default Container;
