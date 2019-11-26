import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { tablet } from "../../utils/device";
import Label from "../label";
import HelpButton from "../help-button";
import { Text } from "../text";

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
      line-height: 24px;
      margin: 2px 0 0 4px;
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
      line-height: unset;
      margin: 0 0 4px 0;
    }
    .field-label-icon {
      display: inline-flex;
      width: 100%;
    }
    .field-body {
      width: 100%;
    }
    .icon-button {
      position: relative;
      line-height: unset;
      margin: -4px 0 0 4px;
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
            <Text.Body className="error-label" fontSize={10} color={errorColor}>
              {errorMessage}
            </Text.Body>
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
  errorMessageWidth: PropTypes.string
};

FieldContainer.defaultProps = {
  place: "bottom",
  maxLabelWidth: "110px",
  errorColor: "#C96C27",
  errorMessageWidth: "293px"
};

export default FieldContainer;
