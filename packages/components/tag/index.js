import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import CrossIconReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import TagIconReactSvgUrl from "PUBLIC_DIR/images/tag.react.svg?url";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import IconButton from "../icon-button";
import Text from "../text";

import {
  StyledTag,
  StyledDropdownIcon,
  StyledDropdownText,
} from "./styled-tag";

const Tag = ({
  tag,
  label,
  isNewTag,
  isDisabled,
  isDefault,
  isLast,
  onDelete,
  onClick,
  advancedOptions,
  tagMaxWidth,
  id,
  className,
  style,
  icon,
}) => {
  const [openDropdown, setOpenDropdown] = React.useState(false);

  const tagRef = React.useRef(null);
  const isMountedRef = React.useRef(true);
  const onClickOutside = React.useCallback((e) => {
    if (e?.target?.className?.includes("advanced-tag") || !isMountedRef.current)
      return;

    setOpenDropdown(false);
  }, []);

  React.useEffect(() => {
    if (openDropdown) {
      return document.addEventListener("click", onClickOutside);
    }

    document.removeEventListener("click", onClickOutside);
    return () => {
      document.removeEventListener("click", onClickOutside);
    };
  }, [openDropdown, onClickOutside]);

  React.useEffect(() => {
    return () => {
      isMountedRef.current = false;
    };
  }, []);

  const openDropdownAction = (e) => {
    if (e?.target?.className?.includes("backdrop-active")) return;

    setOpenDropdown(true);
  };

  const onClickAction = React.useCallback(
    (e) => {
      if (onClick && !isDisabled) {
        onClick(e.target.dataset.tag);
      }
    },
    [onClick, isDisabled]
  );

  const onDeleteAction = React.useCallback(
    (e) => {
      if (e.target != tagRef.current && onDelete) {
        onDelete && onDelete(tag);
      }
    },
    [onDelete, tag, tagRef]
  );

  return (
    <>
      {!!advancedOptions ? (
        <>
          <StyledTag
            id={id}
            className={`tag advanced-tag ${className ? ` ${className}` : ""}`}
            style={style}
            ref={tagRef}
            onClick={openDropdownAction}
            isDisabled={isDisabled}
            isDefault={isDefault}
            isLast={isLast}
            tagMaxWidth={tagMaxWidth}
            isClickable={!!onClick}
          >
            <Text className={"tag-text"} font-size={"13px"} noSelect>
              ...
            </Text>
          </StyledTag>
          <DropDown
            open={openDropdown}
            forwardedRef={tagRef}
            clickOutsideAction={onClickOutside}
            // directionX={"right"}
            manualY={"4px"}
          >
            {advancedOptions.map((tag, index) => (
              <DropDownItem
                className="tag__dropdown-item tag"
                key={`${tag}_${index}`}
                onClick={onClickAction}
                data-tag={tag}
              >
                <StyledDropdownIcon
                  className="tag__dropdown-item-icon"
                  src={TagIconReactSvgUrl}
                />
                <StyledDropdownText
                  className="tag__dropdown-item-text"
                  fontWeight={600}
                  fontSize={"12px"}
                  truncate
                >
                  {tag}
                </StyledDropdownText>
              </DropDownItem>
            ))}
          </DropDown>
        </>
      ) : (
        <StyledTag
          title={label}
          onClick={onClickAction}
          isNewTag={isNewTag}
          isDisabled={isDisabled}
          isDefault={isDefault}
          tagMaxWidth={tagMaxWidth}
          data-tag={label}
          id={id}
          className={`tag${className ? ` ${className}` : ""}`}
          style={style}
          isLast={isLast}
          isClickable={!!onClick}
        >
          {icon ? (
            <ReactSVG className="third-party-tag" src={icon} />
          ) : (
            <>
              <Text
                className={"tag-text"}
                title={label}
                font-size={"13px"}
                noSelect
                truncate
              >
                {label}
              </Text>
              {isNewTag && (
                <IconButton
                  className={"tag-icon"}
                  iconName={CrossIconReactSvgUrl}
                  size={"10px"}
                  onClick={onDeleteAction}
                />
              )}
            </>
          )}
        </StyledTag>
      )}
    </>
  );
};

Tag.propTypes = {
  /** Accepts the tag id */
  tag: PropTypes.string,
  /** Accepts the tag label */
  label: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Accepts the tag styles as new and adds the delete button */
  isNewTag: PropTypes.bool,
  /** Accepts the tag styles as disabled and disables clicking */
  isDisabled: PropTypes.bool,
  /** Accepts the function that is called when the tag is clicked */
  onClick: PropTypes.func,
  /** Accepts the function that ist called when the tag delete button is clicked */
  onDelete: PropTypes.func,
  /** Accepts the max width of the tag */
  tagMaxWidth: PropTypes.string,
  /** Accepts the dropdown options */
  advancedOptions: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default React.memo(Tag);
