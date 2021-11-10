import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import styled from "styled-components";

import Text from "../../text";
import {
  StyledArrowIcon,
  StyledIcon,
  StyledOptionalItem,
  StyledComboButton,
} from "./styled-combobutton";

import ExpanderDownIcon from "../../../../public/images/expander-down.react.svg";
import commonIconsStyles from "../../utils/common-icons-style";

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
`;
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
      comboIcon,
    } = this.props;

    const defaultOption = selectedOption.default;

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
            isDisabled={isDisabled}
            defaultOption={defaultOption}
          >
            {innerContainer}
          </StyledOptionalItem>
        )}
        {selectedOption && selectedOption.icon && (
          <StyledIcon
            className="forceColor"
            isDisabled={isDisabled}
            defaultOption={defaultOption}
          >
            <ReactSVG
              src={selectedOption.icon}
              className="combo-button_selected-icon"
            />
          </StyledIcon>
        )}
        <Text
          noBorder={noBorder}
          title={selectedOption.label}
          as="div"
          truncate={true}
          fontWeight={600}
          className="combo-button-label"
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
            (comboIcon ? (
              <ReactSVG
                src={comboIcon}
                className="custom-combo-buttons_expander-icon"
              />
            ) : (
              <StyledExpanderDownIcon
                size="scale"
                className="combo-buttons_expander-icon"
              />
            ))}
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
  comboIcon: PropTypes.string,
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
