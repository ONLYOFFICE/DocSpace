import React from "react";
import { Link } from "react-router-dom";
import PropTypes from "prop-types";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import NoUserSelect from "@docspace/components/utils/commonStyles";
const LogoItem = styled.div`
  display: flex;
  min-width: 48px;
  min-height: 48px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;

  .nav-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    ${NoUserSelect}
  }

  .nav-logo-icon {
    display: ${(props) => (props.opened ? "block" : "none")};
  }
`;

const NavLogoItem = (props) => {
  //console.log("NavLogoItem render");
  return (
    <LogoItem opened={props.opened}>
      <Link className="nav-logo-wrapper" to="/" onClick={props.onClick}>
        <img className="nav-logo-icon" src={props.logoUrl} />
      </Link>
    </LogoItem>
  );
};

NavLogoItem.displayName = "NavLogoItem";

NavLogoItem.propTypes = {
  opened: PropTypes.bool,
  onClick: PropTypes.func,
  logoUrl: PropTypes.string,
};

export default inject(({ auth }) => ({
  logoUrl: auth.settingsStore.logoUrl,
}))(observer(NavLogoItem));
