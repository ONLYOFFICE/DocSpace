import React from "react";
import PropTypes from "prop-types";

import { ReactSVG } from "react-svg";

import Text from "../text";

import Badge from "../badge";

import {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemSibling,
  StyledCatalogItemBadgeWrapper,
  StyledCatalogItemText,
  StyledCatalogItemInitialText,
  StyledCatalogItemHeaderContainer,
} from "./styled-catalog-item";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
const getInitial = (text) => text.substring(0, 1).toUpperCase();

const CatalogItem = (props) => {
  const {
    className,
    id,
    style,
    icon,
    text,
    showText,
    onClick,
    onDrop,
    isEndOfBlock,
    isActive,
    isDragging,
    isDragActive,
    showInitial,
    showBadge,
    labelBadge,
    iconBadge,
    onClickBadge,
    isHeader,
    isFirstHeader,
  } = props;

  const onClickAction = () => {
    onClick && onClick(id);
  };

  const onClickBadgeAction = (e) => {
    console.log("PRO", onClickBadge);
    e.stopPropagation();
    onClickBadge && onClickBadge(id);
  };

  const onMouseUpAction = () => {
    onDrop && isDragging && onDrop(id, text);
  };

  const renderHeaderItem = () => {
    return (
      <StyledCatalogItemHeaderContainer
        showText={showText}
        isFirstHeader={isFirstHeader}
      >
        <Text className="catalog-item__header-text" truncate noSelect>
          {showText ? text : ""}
        </Text>
      </StyledCatalogItemHeaderContainer>
    );
  };

  const renderItem = () => {
    return (
      <ColorTheme
        className={className}
        id={id}
        style={style}
        showText={showText}
        isEndOfBlock={isEndOfBlock}
        isActive={isActive}
        themeId={ThemeType.CatalogItem}
      >
        <StyledCatalogItemSibling
          isActive={isActive}
          isDragging={isDragging}
          isDragActive={isDragActive}
          onClick={onClickAction}
          onMouseUp={onMouseUpAction}
        />

        <StyledCatalogItemImg isActive={isActive}>
          <ReactSVG className="icon" src={icon} />
          {!showText && (
            <>
              {showInitial && (
                <StyledCatalogItemInitialText>
                  {getInitial(text)}
                </StyledCatalogItemInitialText>
              )}
              {showBadge && !iconBadge && (
                <StyledCatalogItemBadgeWrapper
                  onClick={onClickBadgeAction}
                  showText={showText}
                />
              )}
            </>
          )}
        </StyledCatalogItemImg>

        {showText && (
          <StyledCatalogItemText isActive={isActive} noSelect={true}>
            {text}
          </StyledCatalogItemText>
        )}

        {showBadge && showText && (
          <StyledCatalogItemBadgeWrapper
            showText={showText}
            onClick={onClickBadgeAction}
          >
            {!iconBadge ? (
              <Badge className="catalog-item__badge" label={labelBadge} />
            ) : (
              <ReactSVG className="catalog-item__icon" src={iconBadge} />
            )}
          </StyledCatalogItemBadgeWrapper>
        )}
      </ColorTheme>
    );
  };

  return isHeader ? renderHeaderItem() : renderItem();
};

CatalogItem.propTypes = {
  /** Accepts className */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Catalog item icon */
  icon: PropTypes.string,
  /** Catalog item text */
  text: PropTypes.string,
  /** Tells when the catalog item should display text */
  showText: PropTypes.bool,
  /** Call function when user clicked on catalog item */
  onClick: PropTypes.func,
  /** Call function when user mouse up on catalog item with dragging */
  onDrop: PropTypes.func,
  /** Tells when the catalog item should display initial on icon, text should be hidden */
  showInitial: PropTypes.bool,
  /** Tells when the catalog item should be end of block */
  isEndOfBlock: PropTypes.bool,
  /** Tells when the catalog item should be active */
  isActive: PropTypes.bool,
  /** Tells when the catalog item available for drag`n`drop */
  isDragging: PropTypes.bool,
  /** Tells when the catalog item active for drag`n`drop */
  isDragActive: PropTypes.bool,
  /** Tells when the catalog item should display badge */
  showBadge: PropTypes.bool,
  /** Label in catalog item badge */
  labelBadge: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** Special icon for badge, change default badge */
  iconBadge: PropTypes.string,
  /** Call function when user clicked on catalog item badge */
  onClickBadge: PropTypes.func,
  /** Call when catalog item show as header */
  isHeader: PropTypes.bool,
  /** Disable margin top for catalog item header */
  isFirstHeader: PropTypes.bool,
};

CatalogItem.defaultProps = {
  showText: false,
  showBadge: false,
  isActive: false,
  showInitial: false,
  isEndOfBlock: false,
  isDragging: false,
  isDragActive: false,
  isHeader: false,
  isFirstHeader: false,
};

export default React.memo(CatalogItem);
