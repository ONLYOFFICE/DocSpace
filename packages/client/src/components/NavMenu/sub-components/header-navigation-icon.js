import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import Badge from "@docspace/components/badge";
import { Base } from "@docspace/components/themes";

const StyledContainer = styled.div`
  position: relative;
  width: 20px;
  height: 20px;

  display: flex;
  align-items: center;
  justify-content: center;

  margin-right: 22px;

  .navigation-item__svg {
    height: 20px;

    display: flex;
    align-items: center;
    justify-content: center;

    cursor: pointer;

    svg {
      width: 20px;
      height: 20px;
      path {
        fill: ${(props) =>
          props.active
            ? props.theme.header.productColor
            : props.theme.header.linkColor};
      }
    }
  }

  .navigation-item__badge {
    position: absolute;
    top: -8px;
    right: -8px;

    width: 12px;
    height: 12px;

    display: flex;
    align-items: center;
    justify-content: center;

    div {
      width: 2px;
      height: 12px;

      display: flex;
      align-items: center;
      justify-content: center;

      p {
        font-weight: 800;
        font-size: 9px;
        line-height: 12px;
      }
    }
  }
`;

StyledContainer.defaultProps = { theme: Base };

const HeaderNavigationIcon = ({
  id,
  iconUrl,
  link,
  active,
  badgeNumber,
  onItemClick,
  onBadgeClick,
  url,
  ...rest
}) => {
  return (
    <StyledContainer active={active} {...rest}>
      <ReactSVG
        onClick={onItemClick}
        className="navigation-item__svg"
        src={iconUrl}
        {...rest}
      />

      {badgeNumber > 0 && (
        <Badge
          className="navigation-item__badge"
          label={badgeNumber}
          onClick={onBadgeClick}
        />
      )}
    </StyledContainer>
  );
};

HeaderNavigationIcon.propTypes = {
  id: PropTypes.string,
  iconUrl: PropTypes.string,
  link: PropTypes.string,
  active: PropTypes.bool,
  badgeNumber: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  onClick: PropTypes.func,
  onBadgeClick: PropTypes.func,
  url: PropTypes.string,
};

export default React.memo(HeaderNavigationIcon);
