import React, { useState, useRef } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import ContextMenuButton from "@docspace/components/context-menu-button";
import ContextMenu from "@docspace/components/context-menu";
import {
  isTablet as isTabletUtils,
  isMobile as isMobileUtils,
} from "@docspace/components/utils/device";
import { isTablet, isMobileOnly } from "react-device-detect";
import {
  StyledArticleProfile,
  StyledUserName,
  StyledProfileWrapper,
} from "../styled-article";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import DefaultUserPhotoPngUrl from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";

const ArticleProfile = (props) => {
  const { user, showText, getUserRole, getActions, onProfileClick } = props;
  const { t } = useTranslation("Common");
  const [isOpen, setIsOpen] = useState(false);
  const ref = useRef(null);
  const menuRef = useRef(null);

  const isTabletView =
    (isTabletUtils() || isTablet) && !isMobileOnly && !isMobileUtils();
  const avatarSize = isTabletView ? "min" : "base";
  const userRole = getUserRole(user);

  const toggle = (e, isOpen) => {
    isOpen ? menuRef.current.show(e) : menuRef.current.hide(e);
    setIsOpen(isOpen);
  };

  const onAvatarClick = (e) => {
    if (isTabletView && !showText) {
      toggle(e, !isOpen);
    } else {
      onProfileClick();
    }
  };

  const onHide = () => {
    setIsOpen(false);
  };

  const model = getActions(t);
  const username = user.displayName.split(" ");

  const userAvatar = user.hasAvatar ? user.avatar : DefaultUserPhotoPngUrl;

  if (!isMobileOnly && isMobileUtils()) return <></>;

  return (
    <StyledProfileWrapper showText={showText}>
      <StyledArticleProfile showText={showText} tablet={isTabletView}>
        <div ref={ref}>
          <Avatar
            className={"profile-avatar"}
            id="user-avatar"
            size={avatarSize}
            role={"user"}
            source={userAvatar}
            userName={user.displayName}
            onClick={onAvatarClick}
          />
          <ContextMenu
            model={model}
            containerRef={ref}
            ref={menuRef}
            onHide={onHide}
            scaled={false}
            leftOffset={-50}
          />
        </div>
        {(!isTabletView || showText) && (
          <>
            <StyledUserName
              length={user.displayName.length}
              onClick={onProfileClick}
            >
              <Text fontWeight={600} noSelect truncate>
                {username[0]}
                &nbsp;
              </Text>
              <Text fontWeight={600} noSelect truncate>
                {username[1]}
              </Text>
            </StyledUserName>
            <ContextMenuButton
              id="user-option-button"
              className="option-button"
              iconClassName="option-button-icon"
              zIndex={402}
              directionX="left"
              directionY="top"
              iconName={VerticalDotsReactSvgUrl}
              size={32}
              isFill
              getData={() => getActions(t)}
              isDisabled={false}
              usePortal={true}
            />
          </>
        )}
      </StyledArticleProfile>
    </StyledProfileWrapper>
  );
};

export default inject(({ auth, profileActionsStore }) => {
  const { getActions, getUserRole, onProfileClick } = profileActionsStore;

  return {
    onProfileClick,
    user: auth.userStore.user,
    getUserRole,
    getActions,
  };
})(observer(ArticleProfile));
