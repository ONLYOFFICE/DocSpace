import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import Text from "@docspace/components/text";

const StyledIcon = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: ${(props) => props.size};
  width: ${(props) => props.size};

  .room-background {
    height: ${(props) => props.size};
    width: ${(props) => props.size};
    border-radius: ${(props) => props.radius};
    vertical-align: middle;
    background: ${(props) =>
      props.isArchive
        ? props.theme.roomIcon.backgroundArchive
        : `#` + props.color};
    position: absolute;
    opacity: ${(props) => props.theme.roomIcon.opacityBackground};
  }

  .room-title {
    font-size: 14px;
    font-weight: 700;
    line-height: 16px;
    color: #ffffff;
    position: relative;
    ${(props) =>
      !props.theme.isBase &&
      !props.isArchive &&
      css`
        color: ${(props) => `#` + props.color};
      `};
  }
`;

StyledIcon.defaultProps = { theme: Base };

const RoomIcon = ({
  title,
  isArchive,
  color,
  size = "32px",
  radius = "6px",
}) => {
  const titleWithoutSpaces = title.replace(/\s+/g, " ").trim();
  const indexAfterLastSpace = titleWithoutSpaces.lastIndexOf(" ");
  const secondCharacter =
    indexAfterLastSpace === -1
      ? ""
      : titleWithoutSpaces[indexAfterLastSpace + 1];

  const roomTitle = (title[0] + secondCharacter).toUpperCase();

  return (
    <StyledIcon color={color} size={size} radius={radius} isArchive={isArchive}>
      <div className="room-background" />
      <Text className="room-title">{roomTitle}</Text>
    </StyledIcon>
  );
};

export default RoomIcon;
