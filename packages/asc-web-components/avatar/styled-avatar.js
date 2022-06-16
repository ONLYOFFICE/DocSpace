import styled from "styled-components";

import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";
import { CameraIcon } from "./svg";
import commonIconsStyles from "../utils/common-icons-style";

const EmptyIcon = styled(CameraIcon)`
  ${commonIconsStyles}
  border-radius: ${(props) => props.theme.avatar.image.borderRadius};
`;
EmptyIcon.defaultProps = { theme: Base };

const EditContainer = styled.div`
  position: absolute;
  display: flex;
  right: ${(props) => props.theme.avatar.editContainer.right};
  bottom: ${(props) => props.theme.avatar.editContainer.bottom};
  background-color: ${(props) =>
    props.theme.avatar.editContainer.backgroundColor};
  border-radius: ${(props) => props.theme.avatar.editContainer.borderRadius};
  height: ${(props) => props.theme.avatar.editContainer.height};
  width: ${(props) => props.theme.avatar.editContainer.width};
  align-items: center;
  justify-content: center;

  .edit_icon {
    svg {
      path {
        fill: ${(props) => props.theme.avatar.editContainer.fill};
      }
    }
  }
`;
EditContainer.defaultProps = { theme: Base };

const AvatarWrapper = styled.div`
  border-radius: ${(props) => props.theme.avatar.imageContainer.borderRadius};
  height: ${(props) => props.theme.avatar.imageContainer.height};

  background-color: ${(props) =>
    props.source
      ? props.theme.avatar.icon.background
      : props.userName
      ? props.theme.avatar.imageContainer.backgroundImage
      : props.theme.avatar.imageContainer.background};

  & > svg {
    display: ${(props) => props.theme.avatar.imageContainer.svg.display};
    width: ${(props) => props.theme.avatar.imageContainer.svg.width} !important;
    height: ${(props) =>
      props.theme.avatar.imageContainer.svg.height} !important;
    margin: ${(props) => props.theme.avatar.imageContainer.svg.margin};
    path {
      fill: ${(props) => props.theme.avatar.imageContainer.svg.fill};
    }
  }
`;
AvatarWrapper.defaultProps = { theme: Base };

const leftStyle = (props) =>
  props.theme.avatar.roleWrapperContainer.left[props.size];
const bottomStyle = (props) =>
  props.theme.avatar.roleWrapperContainer.bottom[props.size];

const RoleWrapper = styled.div`
  position: absolute;
  left: ${(props) => leftStyle(props)};
  bottom: ${(props) => bottomStyle(props)};

  height: ${(props) =>
    (props.size === "max" &&
      props.theme.avatar.roleWrapperContainer.height.max) ||
    (props.size === "medium" &&
      props.theme.avatar.roleWrapperContainer.height.medium) ||
    "12px"};
  width: ${(props) =>
    (props.size === "max" &&
      props.theme.avatar.roleWrapperContainer.width.max) ||
    (props.size === "medium" &&
      props.theme.avatar.roleWrapperContainer.width.medium) ||
    "12px"};
`;
RoleWrapper.defaultProps = { theme: Base };

const fontSizeStyle = (props) =>
  props.theme.avatar.initialsContainer.fontSize[props.size];

const NamedAvatar = styled.div`
  position: absolute;
  color: ${(props) => props.theme.avatar.initialsContainer.color};
  left: ${(props) => props.theme.avatar.initialsContainer.left};
  top: ${(props) => props.theme.avatar.initialsContainer.top};
  transform: ${(props) => props.theme.avatar.initialsContainer.transform};
  font-weight: ${(props) => props.theme.avatar.initialsContainer.fontWeight};
  font-size: ${(props) => fontSizeStyle(props)};

  ${NoUserSelect}
`;
NamedAvatar.defaultProps = { theme: Base };

const StyledImage = styled.img`
  width: ${(props) => props.theme.avatar.image.width};
  height: ${(props) => props.theme.avatar.image.height};
  border-radius: ${(props) => props.theme.avatar.image.borderRadius};
  content: ${(props) => props.isDefault && props.theme.avatar.defaultImage};
  ${NoUserSelect};
`;
StyledImage.defaultProps = { theme: Base };

const StyledIconWrapper = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;

  .icon,
  .icon:hover {
    width: 50%;
    height: 50%;
    path {
      fill: ${(props) => props.theme.avatar.icon.color};
    }
  }
`;
StyledIconWrapper.defaultProps = { theme: Base };

const widthStyle = (props) => props.theme.avatar.width[props.size];
const heightStyle = (props) => props.theme.avatar.height[props.size];

const StyledAvatar = styled.div`
  position: relative;
  width: ${(props) => widthStyle(props)};
  height: ${(props) => heightStyle(props)};
  font-family: ${(props) => props.theme.fontFamily};
  font-style: normal;

  .admin_icon {
    rect:nth-child(1) {
      fill: ${(props) => props.theme.avatar.administrator.fill};
    }
    rect:nth-child(2) {
      stroke: ${(props) => props.theme.avatar.administrator.stroke};
    }
    path {
      fill: ${(props) => props.theme.avatar.administrator.color};
    }
  }

  .guest_icon {
    rect:nth-child(1) {
      fill: ${(props) => props.theme.avatar.guest.fill};
    }
    rect:nth-child(2) {
      stroke: ${(props) => props.theme.avatar.guest.stroke};
    }
    path {
      fill: ${(props) => props.theme.avatar.guest.color};
    }
  }

  .owner_icon {
    rect:nth-child(1) {
      fill: ${(props) => props.theme.avatar.owner.fill};
    }
    rect:nth-child(2) {
      stroke: ${(props) => props.theme.avatar.owner.stroke};
    }
    path {
      fill: ${(props) => props.theme.avatar.owner.color};
    }
  }
`;
StyledAvatar.defaultProps = { theme: Base };

export {
  EmptyIcon,
  EditContainer,
  AvatarWrapper,
  RoleWrapper,
  NamedAvatar,
  StyledImage,
  StyledIconWrapper,
  StyledAvatar,
};
