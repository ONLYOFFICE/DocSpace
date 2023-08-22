import React from "react";

const NavigationLogo = ({ logo, burgerLogo, ...rest }) => {
  return (
    <div {...rest}>
      <img className="logo-icon_svg" src={logo} />
      <div className="header-burger">
        <img src={burgerLogo} /* onClick={onLogoClick} */ />
      </div>
      <div className="header_separator" />
    </div>
  );
};

export default NavigationLogo;
