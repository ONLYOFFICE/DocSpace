import React from "react";
import PropTypes from "prop-types";

import { ReactSVG } from "react-svg";

import Badge from "../badge/";

import {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemSibling,
  StyledCatalogItemBadgeWrapper,
  StyledCatalogItemText,
  StyledCatalogItemInitialText,
} from "./styled-catalog-item";

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
    isEndOfBlock,
    isActive,
    showInitial,
    showBadge,
    labelBadge,
    iconBadge,
    onClickBadge,
  } = props;

  const onClickAction = () => {
    onClick && onClick(id);
  };

  const onClickBadgeAction = (e) => {
    e.stopPropagation();
    onClickBadge && onClickBadge(id);
  };

  return (
    <StyledCatalogItemContainer
      className={className}
      id={id}
      style={style}
      showText={showText}
      isEndOfBlock={isEndOfBlock}
    >
      <StyledCatalogItemSibling
        isActive={isActive}
        onClick={onClickAction}
      ></StyledCatalogItemSibling>

      <StyledCatalogItemImg>
        <ReactSVG src={icon} />
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

      {showText && <StyledCatalogItemText>{text}</StyledCatalogItemText>}

      {showBadge && showText && (
        <StyledCatalogItemBadgeWrapper
          showText={showText}
          onClick={onClickBadgeAction}
        >
          {!iconBadge ? (
            <Badge className="catalog-item__badge" label={labelBadge} />
          ) : (
            <ReactSVG src={iconBadge} />
          )}
        </StyledCatalogItemBadgeWrapper>
      )}
    </StyledCatalogItemContainer>
  );
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
  /** Tells when the catalog item should display initial on icon, text should be hidden */
  showInitial: PropTypes.bool,
  /** Tells when the catalog item should be end of block */
  isEndOfBlock: PropTypes.bool,
  /** Tells when the catalog item should be active */
  isActive: PropTypes.bool,
  /** Tells when the catalog item should display badge */
  showBadge: PropTypes.bool,
  /** Label in catalog item badge */
  labelBadge: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** Special icon for badge, change default badge */
  iconBadge: PropTypes.string,
  /** Call function when user clicked on catalog item badge */
  onClickBadge: PropTypes.func,
};

CatalogItem.defaultProps = {
  showText: false,
  showBadge: false,
  isActive: false,
  showInitial: false,
  isEndOfBlock: false,
};

export default React.memo(CatalogItem);
