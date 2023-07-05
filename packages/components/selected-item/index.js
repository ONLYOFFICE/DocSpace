import React from "react";
import CrossReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import { StyledSelectedItem } from "./styled-selected-item";
import PropTypes from "prop-types";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

const SelectedItem = (props) => {
  const {
    label,
    onClose,
    isDisabled,
    onClick,
    isInline,
    className,
    id,
    propKey,
    group,
  } = props;
  if (!label) return <></>;

  const onCloseClick = (e) => {
    !isDisabled && onClose && onClose(propKey, label, group, e);
  };

  const handleOnClick = (e) => {
    !isDisabled &&
      onClick &&
      !e.target.classList.contains("selected-tag-removed") &&
      onClick(propKey, label, group, e);
  };

  return (
    <StyledSelectedItem
      onClick={handleOnClick}
      isInline={isInline}
      className={className}
      isDisabled={isDisabled}
      id={id}
    >
      <Text
        className={"selected-item_label"}
        title={label}
        truncate={true}
        noSelect
        isDisabled={isDisabled}
      >
        {label}
      </Text>
      <IconButton
        className="selected-tag-removed"
        iconName={CrossReactSvgUrl}
        size={12}
        onClick={onCloseClick}
        isFill
        isDisabled={isDisabled}
      />
    </StyledSelectedItem>
  );
};

SelectedItem.propTypes = {
  /** Selected item text */
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  /** Sets the 'width: fit-content' property */
  isInline: PropTypes.bool,
  /** Sets a callback function that is triggered when the cross icon is clicked */
  onClose: PropTypes.func.isRequired,
  /** Sets a callback function that is triggered when the selected item is clicked */
  onClick: PropTypes.func,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Accepts class  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Accepts key to remove item */
  propKey: PropTypes.string,
  /** Accepts group key to remove item */
  group: PropTypes.string,
};

SelectedItem.defaultProps = {
  isInline: true,
  isDisabled: false,
};

export default React.memo(SelectedItem);
