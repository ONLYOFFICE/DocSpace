import React, { useState, useRef } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
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
  const iconRef = useRef(null);
  const buttonMenuRef = useRef(null);
  const menuRef = useRef(null);

  const isTabletView =
    (isTabletUtils() || isTablet) && !isMobileOnly && !isMobileUtils();
  const avatarSize = isTabletView ? "min" : "base";
  const userRole = getUserRole(user);

  const toggle = (e, isOpen, ref) => {
    isOpen ? ref.current.show(e) : ref.current.hide(e);
    setIsOpen(isOpen);
  };

  const onClick = (e) => toggle(e, !isOpen, buttonMenuRef);

  const onAvatarClick = (e) => {
    if (isTabletView && !showText) {
      toggle(e, !isOpen, menuRef);
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
            <div ref={iconRef}>
              <IconButton
                onClick={onClick}
                iconName={VerticalDotsReactSvgUrl}
                size={15}
                isFill
              />
              <ContextMenu
                model={model}
                containerRef={iconRef}
                ref={buttonMenuRef}
                onHide={onHide}
                scaled={false}
                leftOffset={10}
                topOffset={15}
              />
            </div>
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
