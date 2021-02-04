import React from "react";
import PropTypes from "prop-types";

import { Icons } from "../../icons";
import Text from "../../text";
import {
  StyledArrowIcon,
  StyledIcon,
  StyledOptionalItem,
  StyledComboButton,
} from "./styled-combobutton";

class ComboButton extends React.Component {
  render() {
    const {
      noBorder,
      onClick,
      isDisabled,
      innerContainer,
      innerContainerClassName,
      selectedOption,
      optionsLength,
      withOptions,
      withAdvancedOptions,
      isOpen,
      scaled,
      size,
    } = this.props;

    const boxIconColor = isDisabled ? "#D0D5DA" : "#333333";
    const arrowIconColor = isDisabled ? "#D0D5DA" : "#A3A9AE";
    const defaultIconColor = selectedOption.default
      ? arrowIconColor
      : boxIconColor;

    return (
      <StyledComboButton
        isOpen={isOpen}
        isDisabled={isDisabled}
        noBorder={noBorder}
        containOptions={optionsLength}
        withAdvancedOptions={withAdvancedOptions}
        onClick={onClick}
        scaled={scaled}
        size={size}
        className="combo-button"
      >
        {innerContainer && (
          <StyledOptionalItem
            className={innerContainerClassName}
            color={defaultIconColor}
          >
            {innerContainer}
          </StyledOptionalItem>
        )}
        {selectedOption && selectedOption.icon && (
          <StyledIcon className="forceColor">
            {React.createElement(Icons[selectedOption.icon], {
              size: "scale",
              color: defaultIconColor,
              isfill: true,
            })}
          </StyledIcon>
        )}
        <Text
          noBorder={noBorder}
          title={selectedOption.label}
          as="div"
          truncate={true}
          fontWeight={600}
          className="combo-button-label"
          //color={selectedOption.default ? arrowIconColor +' !important' : boxIconColor}
        >
          {selectedOption.label}
        </Text>
        <StyledArrowIcon
          needDisplay={withOptions || withAdvancedOptions}
          noBorder={noBorder}
          isOpen={isOpen}
          className="combo-buttons_arrow-icon"
        >
          {(withOptions || withAdvancedOptions) &&
            React.createElement(Icons["ExpanderDownIcon"], {
              size: "scale",
              color:
                selectedOption.arrowIconColor && !isDisabled
                  ? selectedOption.arrowIconColor
                  : arrowIconColor,
              isfill: true,
            })}
        </StyledArrowIcon>
      </StyledComboButton>
    );
  }
}

ComboButton.propTypes = {
  noBorder: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedOption: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.object),
    PropTypes.object,
  ]),
  withOptions: PropTypes.bool,
  optionsLength: PropTypes.number,
  withAdvancedOptions: PropTypes.bool,
  innerContainer: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  innerContainerClassName: PropTypes.string,
  isOpen: PropTypes.bool,
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "content"]),
  scaled: PropTypes.bool,
  onClick: PropTypes.func,
};

ComboButton.defaultProps = {
  noBorder: false,
  isDisabled: false,
  withOptions: true,
  withAdvancedOptions: false,
  innerContainerClassName: "innerContainer",
  isOpen: false,
  size: "content",
  scaled: false,
};

export default ComboButton;
