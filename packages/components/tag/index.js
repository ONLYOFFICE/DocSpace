import React from "react";
import { ReactSVG } from "react-svg";

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

  const onClickOutside = React.useCallback((e) => {
    if (e?.target?.className?.includes("advanced-tag") || !isMountedRef.current)
      return;

    setOpenDropdown(false);
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
                  src="/static/images/tag.react.svg"
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
                  iconName={"/static/images/cross.react.svg"}
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

export default React.memo(Tag);
