import React from "react";
import PropTypes from "prop-types";

import Label from "../label";
import HelpButton from "../help-button";
import Text from "../text";
import Container from "./styled-field-container";

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
      labelVisible,
      labelText,
      children,
      tooltipContent,
      place,
      helpButtonHeaderContent,
      maxLabelWidth,
      errorMessage,
      errorColor,
      errorMessageWidth,
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
        {labelVisible && (
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
        )}
        <div className="field-body">
          {children}
          {hasError ? (
            <Text className="error-label" fontSize="10px" color={errorColor}>
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
  labelVisible: PropTypes.bool,
  labelText: PropTypes.string,
  icon: PropTypes.string,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  place: PropTypes.string,
  helpButtonHeaderContent: PropTypes.string,
  maxLabelWidth: PropTypes.string,
  errorMessage: PropTypes.string,
  errorColor: PropTypes.string,
  errorMessageWidth: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

FieldContainer.defaultProps = {
  place: "bottom",
  labelVisible: true,
  maxLabelWidth: "110px",
  errorColor: "#C96C27",
  errorMessageWidth: "293px",
};

export default FieldContainer;
