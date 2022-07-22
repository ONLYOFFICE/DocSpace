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
      offsetRight,
      tooltipMaxWidth,
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
                offsetRight={offsetRight}
                helpButtonHeaderContent={helpButtonHeaderContent}
                tooltipMaxWidth={tooltipMaxWidth}
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
  /** Vertical or horizontal alignment */
  isVertical: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Indicates that the field is required to fill */
  isRequired: PropTypes.bool,
  /** Indicates that the field is incorrect */
  hasError: PropTypes.bool,
  /** Sets visibility of field label section */
  labelVisible: PropTypes.bool,
  /** Field label text */
  labelText: PropTypes.string,
  icon: PropTypes.string,
  /** Children elements */
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  /** Tooltip content */
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  place: PropTypes.string,
  /** Tooltip header content (tooltip opened in aside) */
  helpButtonHeaderContent: PropTypes.string,
  /** Max label width in horizontal alignment */
  maxLabelWidth: PropTypes.string,
  /** Error message text */
  errorMessage: PropTypes.string,
  /** Error text color */
  errorColor: PropTypes.string,
  /** Error text width */
  errorMessageWidth: PropTypes.string,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  offsetRight: PropTypes.number,
  tooltipMaxWidth: PropTypes.string,
};

FieldContainer.defaultProps = {
  place: "bottom",
  labelVisible: true,
  maxLabelWidth: "110px",
  errorMessageWidth: "293px",
  offsetRight: 0,
};

export default FieldContainer;
