import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import roomsIconsColors from "@docspace/components/utils/roomsIconsColors";
import Text from "@docspace/components/text";

const StyledIcon = styled.div`
  ${NoUserSelect}
  display: flex;
  justify-content: center;
  align-items: center;
  height: ${(props) => props.size + `px`};
  width: ${(props) => props.size + `px`};

  .room-background {
    height: ${(props) => props.size + `px`};
    width: ${(props) => props.size + `px`};
    border-radius: 6px;
    vertical-align: middle;
    background: ${(props) =>
      props.isArchive
        ? props.theme.isBase
          ? "#A3A9AE"
          : "#FFFFFF"
        : props.color};
    position: absolute;
    ${(props) =>
      !props.theme.isBase &&
      css`
        opacity: 0.1;
      `};
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
        color: ${(props) => props.color};
      `};
  }
`;

StyledIcon.defaultProps = { theme: Base };

const RoomIcon = ({ title, isArchive, size = 32 }) => {
  const randomPropertyValue = (object) => {
    const keys = Object.keys(object);
    if (keys.length > 0) {
      const index = Math.floor(keys.length * Math.random());
      return object[keys[index]];
    }
    return null;
  };

  const titleWithoutTooManySpaces = title.replace(/\s+/g, " ").trim();
  const indexSecondCharacterAfterSpace = titleWithoutTooManySpaces.indexOf(" ");
  const secondCharacterAfterSpace =
    indexSecondCharacterAfterSpace === -1
      ? ""
      : titleWithoutTooManySpaces[indexSecondCharacterAfterSpace + 1];

  const roomTitle = (title[0] + secondCharacterAfterSpace).toUpperCase();

  const color = randomPropertyValue(roomsIconsColors);

  return (
    <StyledIcon color={color} size={size} isArchive={isArchive}>
      <div className="room-background" />
      <Text className="room-title">{roomTitle}</Text>
    </StyledIcon>
  );
};

export default RoomIcon;
