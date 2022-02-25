import React, { useState } from "react";
import { ReactSVG } from "react-svg";
import Badge from "@appserver/components/badge";
import Link from "@appserver/components/link";
import styled from "styled-components";
import { getLink, onItemClick } from "../../../../src/helpers/utils";

// const separatorColor = "#3E668D";

// const StyledNavDesktopItemSeparator = styled.div`
//   height: 20px;
//   margin: 0px 2px 0 20px;
//   width: 1px;
//   background: #3e668d;
//   border-left: 1px solid ${separatorColor};
// `;

const StyledNavDesktopItem = styled(Link)`
  cursor: pointer;
  display: flex;
  margin-left: 16px;

  .popup-icon {
    svg {
      margin-top: 4px;
      path {
        fill: ${(props) => (props.isActive ? "#fff" : "#7A95B0")};
      }
    }
  }
`;

const StyledBadge = styled(Badge)`
  position: absolute;
  height: 16px;
  margin-top: -8px;
  margin-left: 8px;
`;

const NavDesktopItem = ({ module, isActive }, ...rest) => {
  const { iconUrl, notifications, link, separator, title } = module;
  const [showPopup, setShowPopup] = useState(false);
  const onBadgeClick = () => setShowPopup(!showPopup);
  const dataLink = getLink(link);

  // <StyledNavDesktopItemSeparator {...rest} />
  return separator ? (
    <></>
  ) : (
    <StyledNavDesktopItem isActive={isActive} href={link} {...rest}>
      <>
        <ReactSVG
          className="popup-icon"
          onClick={onItemClick}
          src={iconUrl}
          data-link={dataLink}
        />
        <StyledBadge label={notifications} onClick={onBadgeClick} />
      </>
    </StyledNavDesktopItem>
  );
};

export default NavDesktopItem;
