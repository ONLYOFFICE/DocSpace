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
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
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
    folderId,
    badgeTitle,
  } = props;

  const onClickAction = () => {
    onClick && onClick(id);
  };

  const onClickBadgeAction = (e) => {
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
        style={style}
        showText={showText}
        isEndOfBlock={isEndOfBlock}
        isActive={isActive}
        themeId={ThemeType.CatalogItem}
      >
        <StyledCatalogItemSibling
          id={folderId}
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
            title={badgeTitle}
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
  /** Sets the catalog item to display text */
  showText: PropTypes.bool,
  /** Invokes a function upon clicking on a catalog item */
  onClick: PropTypes.func,
  /** Invokes a function upon dragging and dropping a catalog item */
  onDrop: PropTypes.func,
  /** Tells when the catalog item should display initial on icon, text should be hidden */
  showInitial: PropTypes.bool,
  /** Sets the catalog item as end of block */
  isEndOfBlock: PropTypes.bool,
  /** Sets catalog item active */
  isActive: PropTypes.bool,
  /** Sets the catalog item available for drag`n`drop */
  isDragging: PropTypes.bool,
  /** Sets the catalog item active for drag`n`drop */
  isDragActive: PropTypes.bool,
  /** Sets the catalog item to display badge */
  showBadge: PropTypes.bool,
  /** Label in catalog item badge */
  labelBadge: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** Sets custom badge icon */
  iconBadge: PropTypes.string,
  /** Invokes a function upon clicking on the catalog item badge */
  onClickBadge: PropTypes.func,
  /** Sets the catalog item to be displayed as a header */
  isHeader: PropTypes.bool,
  /** Disables margin top for catalog item header */
  isFirstHeader: PropTypes.bool,
  /** Accepts folder id */
  folderId: PropTypes.string,
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
