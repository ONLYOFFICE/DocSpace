import React from "react";
import PropTypes from "prop-types";

import Base from "../themes/base";

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

const getInitial = (text) => text.substring(0, 1);

const CatalogItem = (props) => {
  // console.log("render");

  const {
    className,
    id,
    style,
    icon,
    text,
    showText,
    onClick,
    isEndOfBlock,
    showInitial,
    showBadge,
    labelBadge,
    iconBadge,
    onClickBadge,
  } = props;

  const onClickAction = (e) => {
    onClick && onClick(e);
  };

  const onClickBadgeAction = (e) => {
    onClickBadge && onClickBadge(e);
  };

  return (
    <StyledCatalogItemContainer
      className={className}
      id={id}
      style={style}
      showText={showText}
      isEndOfBlock={isEndOfBlock}
      theme={Base}
    >
      <StyledCatalogItemSibling
        theme={Base}
        onClick={onClickAction}
      ></StyledCatalogItemSibling>

      <StyledCatalogItemImg theme={Base}>
        <ReactSVG src={icon} />
        {!showText && (
          <>
            {showInitial && (
              <StyledCatalogItemInitialText theme={Base}>
                {getInitial(text)}
              </StyledCatalogItemInitialText>
            )}
            {showBadge && !iconBadge && (
              <StyledCatalogItemBadgeWrapper
                onClick={onClickBadgeAction}
                showText={showText}
                theme={Base}
              ></StyledCatalogItemBadgeWrapper>
            )}
          </>
        )}
      </StyledCatalogItemImg>

      {showText && (
        <StyledCatalogItemText theme={Base}>{text}</StyledCatalogItemText>
      )}

      {showBadge && showText && (
        <StyledCatalogItemBadgeWrapper
          showText={showText}
          onClick={onClickBadgeAction}
          theme={Base}
        >
          {!iconBadge ? (
            <Badge label={labelBadge} />
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
  id: PropTypes.string,
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
  showInitial: false,
  isEndOfBlock: false,
};

export default React.memo(CatalogItem);
