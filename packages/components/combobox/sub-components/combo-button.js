import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import {
  StyledArrowIcon,
  StyledIcon,
  StyledOptionalItem,
  StyledTriangleDownIcon,
  StyledLoader,
} from "./styled-combobutton";

import Text from "../../text";


import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Badge from "@docspace/components/badge";

const ComboButton = (props) => {
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
    fillIcon,
    modernView,
    tabIndex,
    isLoading,
    type,
  } = props;

  const defaultOption = selectedOption?.default;
  const isSelected = selectedOption?.key !== 0;
  const displayArrow = withOptions || withAdvancedOptions;

  return (
    <ColorTheme
      isOpen={isOpen}
      isDisabled={isDisabled}
      noBorder={noBorder}
      containOptions={optionsLength}
      withAdvancedOptions={withAdvancedOptions}
      onClick={onClick}
      scaled={scaled}
      size={size}
      isSelected={isSelected}
      modernView={modernView}
      className="combo-button"
      themeId={ThemeType.ComboButton}
      tabIndex={tabIndex}
      displayArrow={displayArrow}
      isLoading={isLoading}
      type={type}
    >
      {innerContainer && (
        <StyledOptionalItem
          className={innerContainerClassName}
          isDisabled={isDisabled}
          defaultOption={defaultOption}
          isLoading={isLoading}
        >
          {innerContainer}
        </StyledOptionalItem>
      )}
      {selectedOption && selectedOption.icon && (
        <StyledIcon
          className="forceColor"
          isDisabled={isDisabled}
          defaultOption={defaultOption}
          isSelected={isSelected}
          isLoading={isLoading}
        >
          <ReactSVG
            src={selectedOption.icon}
            className={fillIcon ? "combo-button_selected-icon" : ""}
          />
        </StyledIcon>
      )}
      {type === "badge" ? (
        <Badge
          label={selectedOption.label}
          noHover={true}
          color={selectedOption.color}
          backgroundColor={selectedOption.backgroundColor}
          border={`2px solid ${selectedOption.border}`}
          compact={!!selectedOption.border}
        />
      ) : (
        <Text
          noBorder={noBorder}
          title={selectedOption?.label}
          as="div"
          truncate={true}
          fontWeight={600}
          className="combo-button-label"
        >
          {selectedOption?.label}
        </Text>
      )}
      <StyledArrowIcon
        displayArrow={displayArrow}
        noBorder={noBorder}
        isOpen={isOpen}
        modernView={modernView}
        className="combo-buttons_arrow-icon"
        isLoading={isLoading}
      >
        {displayArrow &&
          (comboIcon ? (
            <ReactSVG src={comboIcon} className="combo-buttons_expander-icon" />
          ) : (
            <StyledTriangleDownIcon
              size="scale"
              className="combo-buttons_expander-icon"
            />
          ))}
      </StyledArrowIcon>

      {isLoading && (
        <StyledLoader displaySize={size} type="track" size="20px" />
      )}
    </ColorTheme>
  );
};

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
  fillIcon: PropTypes.bool,
  modernView: PropTypes.bool,
  tabIndex: PropTypes.number,
  isLoading: PropTypes.bool,
  type: PropTypes.oneOf(["badge", null]),
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
  modernView: false,
  tabIndex: -1,
  isLoading: false,
};

export default ComboButton;
