import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { tablet } from "../../utils/device";
import Label from "../label";
import HelpButton from "../help-button";
import Text from "../text";

function getHorizontalCss(labelWidth) {
  return css`
    display: flex;
    flex-direction: row;
    align-items: start;
    margin: 0 0 16px 0;

    .field-label {
      line-height: 32px;
      margin: 0;
      position: relative;
    }
    .field-label-icon {
      display: inline-flex;
      min-width: ${labelWidth};
      width: ${labelWidth};
    }
    .field-body {
      flex-grow: 1;
    }
    .icon-button {
      position: relative;
      margin-top: 10px;
      margin-left: 8px;
    }
  `;
}

function getVerticalCss() {
  return css`
    display: flex;
    flex-direction: column;
    align-items: start;
    margin: 0 0 16px 0;

    .field-label {
      line-height: 13px;
      height: 15px;
      display: inline-block;
    }
    .field-label-icon {
      display: inline-flex;
      width: 100%;
      margin: 0 0 8px 0;
    }
    .field-body {
      width: 100%;
    }
    .icon-button {
      position: relative;
      margin: 0;
      padding: 1px 8px;
      width: 13px;
      height: 13px;
    }
  `;
}

const Container = styled.div`
  .error-label {
    max-width: ${props => (props.maxwidth ? props.maxwidth : "293px")}
  }
  ${props =>
    props.vertical
      ? getVerticalCss()
      : getHorizontalCss(props.maxLabelWidth)}

  @media ${tablet} {
    ${getVerticalCss()}
  }
`;

class FieldContainer extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const {
      isVertical,
      className,
      id,
      style,
      isRequired,
      hasError,
      labelText,
      children,
      tooltipContent,
      place,
      helpButtonHeaderContent,
      maxLabelWidth,
      errorMessage,
      errorColor,
      errorMessageWidth
    } = this.props;

    return (
      <Container
        vertical={isVertical}
        maxLabelWidth={maxLabelWidth}
        className={className}
        id={id}
        style={style}
        maxwidth={errorMessageWidth}
      >
        <div className="field-label-icon">
          <Label
            isRequired={isRequired}
            error={hasError}
            text={labelText}
            truncate={true}
            className="field-label"
          />
          {tooltipContent && (
            <HelpButton
              tooltipContent={tooltipContent}
              place={place}
              helpButtonHeaderContent={helpButtonHeaderContent}
            />
          )}
        </div>
        <div className="field-body">
          {children}
          {hasError ? (
            <Text className="error-label" fontSize='10px' color={errorColor}>
              {errorMessage}
            </Text>
          ) : null}
        </div>
      </Container>
    );
  }
}

FieldContainer.displayName = "FieldContainer";

FieldContainer.propTypes = {
  isVertical: PropTypes.bool,
  className: PropTypes.string,
  isRequired: PropTypes.bool,
  hasError: PropTypes.bool,
  labelText: PropTypes.string,
  icon: PropTypes.string,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  place: PropTypes.string,
  helpButtonHeaderContent: PropTypes.string,
  maxLabelWidth: PropTypes.string,
  errorMessage: PropTypes.string,
  errorColor: PropTypes.string,
  errorMessageWidth: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

FieldContainer.defaultProps = {
  place: "bottom",
  maxLabelWidth: "110px",
  errorColor: "#C96C27",
  errorMessageWidth: "293px"
};

export default FieldContainer;
