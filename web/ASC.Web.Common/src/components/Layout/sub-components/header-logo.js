import React from "react";
import styled from "styled-components";
import PropTypes from 'prop-types';

const HeaderLogoContainer = styled.div`
  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .header-logo-min_icon {
    display: none;
    cursor: pointer;
    width: 24px;
    height: 24px;
    @media (max-width: 620px) {
      padding: 0 12px 0 0;
      display: ${props => props.module && "block"};
    }
  }

  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 4px 20px 0 ${props => (props.logged ? "6px" : "240px")};
    cursor: pointer;

    @media (max-width: 768px) {
        padding: 4px 20px 0 ${props => (props.logged ? "6px" : "144px")};
    }

    @media (max-width: 620px) {
      display: ${props => (props.module ? "none" : "block")};
      padding: 0px 20px 0 6px;
    }

    @media (max-width: 375px) {
        padding: 4px 20px 0 ${props => (props.logged ? "6px" : "32px")};
    }
  }
`;

const HeaderLogo = ({ logged }) => {

    return (
        <HeaderLogoContainer logged={logged}>
            <a className="header-logo-wrapper" href="/">
                <img className="header-logo-min_icon" src="images/nav.logo.react.svg" />
                <img
                    className="header-logo-icon"
                    src="images/nav.logo.opened.react.svg"
                />
            </a>
        </HeaderLogoContainer>
    )
}

HeaderLogo.propTypes = {
    logged: PropTypes.bool.isRequired
};

export default HeaderLogo;
