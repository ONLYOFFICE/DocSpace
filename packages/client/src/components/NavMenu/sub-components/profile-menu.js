import React from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import DropDown from "@docspace/components/drop-down";

import styled, { css } from "styled-components";
import DropDownItem from "@docspace/components/drop-down-item";
import { isMobileOnly } from "react-device-detect";
import { Base } from "@docspace/components/themes";
import { mobile, tablet } from "@docspace/components/utils/device";
import CrossIcon from "@docspace/components/public/static/images/cross.react.svg";
import Portal from "@docspace/components/portal";

const StyledWrapper = styled.div`
  @media (min-width: 428px) {
    .backdrop-active {
      background-color: unset;
      backdrop-filter: unset;
    }
  }
`;

const StyledDropDown = styled(DropDown)`
  z-index: 500 !important;

  top: ${(props) =>
    props.isBannerVisible && props.withPortal ? "134px" : "54px"} !important;
  right: 20px !important;

  @media ${tablet} {
    right: 16px !important;
  }

  @media (max-width: 428px) {
    position: fixed;

    top: unset !important;
    right: 0 !important;
    left: 0 !important;
    bottom: 0 !important;
    width: 100vw;

    border: none !important;

    border-radius: 6px 6px 0px 0px !important;
  }
`;

const StyledControlContainer = styled.div`
  background: ${(props) => props.theme.catalog.control.background};
  width: 24px;
  height: 24px;
  position: absolute;
  top: -34px;
  right: 10px;
  border-radius: 100px;
  cursor: pointer;
  display: none;
  align-items: center;
  justify-content: center;
  z-index: 290;

  @media (max-width: 428px) {
    display: flex;
  }
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 12px;
  height: 12px;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

const commonStyle = css`
  font-family: "Open Sans", sans-serif, Arial;
  font-style: normal;
  color: ${(props) => props.theme.menuContainer.color};
  max-width: 300px;
  @media ${mobile} {
    max-width: calc(100vw - 84px);
  }
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const StyledProfileMenu = styled(DropDownItem)`
  position: relative;
  overflow: visible;
  padding: 0px;
  cursor: pointer;
  display: inline-block;
  margin-top: -6px;
`;

export const MenuContainer = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 12px;
  position: relative;
  height: 76px;
  background: ${(props) => props.theme.menuContainer.background};
  border-radius: 6px 6px 0px 0px;
  padding: 16px;
  cursor: default;
  box-sizing: border-box;

  @media ${mobile} {
    max-width: 100vw;
  }

  .avatar {
    height: 40px;
    width: 40px;
    min-height: 40px;
    min-width: 40px;
  }
`;

MenuContainer.defaultProps = { theme: Base };

export const MainLabelContainer = styled.div`
  font-size: 16px;
  line-height: 28px;

  width: auto;

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  ${commonStyle}
`;

MainLabelContainer.defaultProps = { theme: Base };

export const LabelContainer = styled.div`
  font-weight: normal;
  font-size: 11px;
  line-height: 16px;

  ${commonStyle}
`;

LabelContainer.defaultProps = { theme: Base };

class ProfileMenu extends React.Component {
  constructor(props) {
    super(props);
  }

  renderDropDown = () => {
    const {
      avatarRole,
      avatarSource,
      children,
      className,
      displayName,
      clickOutsideAction,
      open,
      forwardedRef,
      isBannerVisible,
    } = this.props;

    return (
      <StyledDropDown
        className={className}
        directionX="right"
        open={open}
        clickOutsideAction={clickOutsideAction}
        forwardedRef={forwardedRef}
        isDefaultMode={false}
        withBlur={isMobileOnly}
        isBannerVisible={isBannerVisible}
        withPortal={isMobileOnly}
      >
        <StyledProfileMenu>
          <MenuContainer>
            <Avatar
              className="avatar"
              size="medium"
              role={avatarRole}
              source={avatarSource}
              userName={displayName}
            />
            <div>
              <MainLabelContainer>{displayName}</MainLabelContainer>
              <StyledControlContainer onClick={clickOutsideAction}>
                <StyledCrossIcon />
              </StyledControlContainer>
            </div>
          </MenuContainer>
        </StyledProfileMenu>
        {children}
      </StyledDropDown>
    );
  };

  render() {
    const { open } = this.props;

    const element = this.renderDropDown();

    if (isMobileOnly) {
      const root = document.getElementById("root");

      const wrapper = <StyledWrapper>{element}</StyledWrapper>;

      return <>{<Portal element={wrapper} appendTo={root} visible={open} />}</>;
    }

    return <>{element}</>;
  }
}

ProfileMenu.displayName = "ProfileMenu";

ProfileMenu.propTypes = {
  avatarRole: PropTypes.oneOf(["owner", "admin", "guest", "user"]),
  avatarSource: PropTypes.string,
  children: PropTypes.any,
  className: PropTypes.string,
  displayName: PropTypes.string,
  id: PropTypes.string,
  open: PropTypes.bool,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  clickOutsideAction: PropTypes.func,
};

export default inject(({ bannerStore }) => {
  const { isBannerVisible } = bannerStore;

  return { isBannerVisible };
})(observer(ProfileMenu));
