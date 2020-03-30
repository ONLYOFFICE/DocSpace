import React from "react";
import styled from "styled-components";
import { constants } from 'asc-web-common';
import { EmptyScreenContainer, Link } from "asc-web-components";
import { fetchFiles } from "../../../../../store/files/actions";
import store from "../../../../../store/store";
import { string } from "prop-types";

const { FileAction } = constants;

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    max-width: 550px;
  }

  .empty-folder_link {
    margin-right: 8px;
  }

  .empty-folder_container-links {
    display: flex;
    margin: 12px 0;
  }

  .empty-folder_container_up-image {
    margin-right: 8px;
    cursor: pointer;
  }

  .empty-folder_container_plus-image {
    margin: -8px 8px 0 0;
  }
`;

const EmptyFolderContainer = props => {
  const linkStyles = {
    isHovered: true,
    type: "action",
    fontSize: "14px",
    className: "empty-folder_link",
    display: "flex"
  };

  const onCreate = (format) => {
    props.setAction(
      {
        type: FileAction.Create,
        extension: format,
        id: -1
      });
  }

  const onBackToParentFolder = () => {
    const newFilter = props.filter.clone();
    fetchFiles(props.parentId, newFilter, store.dispatch);
  };

  return (
    <EmptyFolderWrapper>
      <EmptyScreenContainer
        className="empty-folder_container"
        imageSrc="images/empty_screen.png"
        imageAlt="Empty folder image"
        headerText="В этой папке нет файлов"
        buttons={
          <>
            <div className="empty-folder_container-links">
              <Link
                className="empty-folder_container_plus-image"
                color="#83888d"
                fontSize="26px"
                fontWeight="800"
                noHover
                onClick={() => console.log("Create document click")}
              >
                +
              </Link>
              <Link onClick={onCreate.bind(this, 'docx')} {...linkStyles}>
                Документ,
              </Link>
              <Link onClick={onCreate.bind(this, 'xlsx')} {...linkStyles}>
                Таблица,
              </Link>
              <Link onClick={onCreate.bind(this, 'pptx')} {...linkStyles}>
                Презентация
              </Link>
            </div>
            <div className="empty-folder_container-links">
              <Link
                className="empty-folder_container_plus-image"
                color="#83888d"
                fontSize="26px"
                fontWeight="800"
                onClick={onCreate.bind(this, null)}
                noHover
              >
                +
              </Link>
              <Link {...linkStyles} onClick={onCreate.bind(this, null)}>
                Папка
              </Link>
            </div>
            <div className="empty-folder_container-links">
              <img
                className="empty-folder_container_up-image"
                src="images/up.svg"
                onClick={onBackToParentFolder}
              />
              <Link onClick={onBackToParentFolder} {...linkStyles}>
                Вернутся в папку на уровень выше
              </Link>
            </div>
          </>
        }
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFolderContainer;
