import React from "react";
import styled, { css } from "styled-components";
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import NoUserSelect from "@docspace/components/utils/commonStyles";

const isRootFolderStyles = css`
  padding: 0 36px 64px 12px;

  @media (min-width: 600px) {
    padding: 40px 68px 64px 93px;
    grid-column-gap: 33px;
  }

  @media (min-width: 1024px) {
    padding: 40px 0px 64px 0px;
    grid-column-gap: 40px;
    grid-template-columns: 100px 1fr;
  }
`;

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    .empty-folder_link {
      margin-right: 7px;
    }

    .empty-folder_container-links {
      display: grid;
      margin: 13px 0;
      grid-template-columns: 12px 1fr;
      grid-column-gap: 8px;
    }

    .flex-wrapper_container {
      display: flex;
      flex-wrap: wrap;
    }

    .empty-folder_container_up-image,
    .empty-folder_container_plus-image {
      margin: 4px 8px 0 0;
      cursor: pointer;
    }

    .empty-folder_container_plus-image {
      display: flex;
      line-height: unset;
      ${NoUserSelect}
    }

    .empty-folder_container-icon {
      height: 20px;
      width: 12px;
      margin: 4px 4px 0 0;
      cursor: pointer;
    }

    .empty-connect_container-links {
      position: relative;
      bottom: 16px;
    }

    ${(props) => props.isRootStyles && `${isRootFolderStyles}`}
  }
`;

const EmptyFoldersContainer = (props) => {
  const imageAlt = "Empty folder image";
  const {
    imageSrc,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
    style,
    imageStyle,
    buttonStyle,
    isRootStyles,
  } = props;

  return (
    <EmptyFolderWrapper isRootStyles={isRootStyles}>
      <EmptyScreenContainer
        className="empty-folder_container"
        style={style}
        imageStyle={imageStyle}
        imageSrc={imageSrc}
        imageAlt={imageAlt}
        buttonStyle={buttonStyle}
        headerText={headerText}
        subheadingText={subheadingText}
        descriptionText={descriptionText}
        buttons={buttons}
        isRootStyles={isRootStyles}
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFoldersContainer;
