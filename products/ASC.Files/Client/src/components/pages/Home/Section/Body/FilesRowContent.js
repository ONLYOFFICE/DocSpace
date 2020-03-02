
import React from "react";
import styled from "styled-components";
import { RowContent, Link, Text, Icons } from "asc-web-components";

const FilesRowContent = (props) => {
  const { title, fileExst, created, createdBy, contentLength, filesCount, foldersCount } = props.item;

  const SimpleFilesRowContent = styled(RowContent)`
    .badge-ext {
      margin-left: -8px;
      margin-right: 8px;
    }

    .badge {
      margin-right: 8px;
      margin-top: ${props => props.isFile ? `-4px` : `0px`}
    }
  `
  const titleWithoutExt = fileExst
    ? title.split('.').slice(0, -1).join('.')
    : title;

  const fileOwner = (props.viewer.id === createdBy.id && "Me") || createdBy.displayName;
  const createdDate = new Date(created).toLocaleString("EN-US");

  return (
    <SimpleFilesRowContent
      sideColor="#333"
      isFile={fileExst}
    >
      <Link
        type='page'
        title={titleWithoutExt}
        fontWeight="bold"
        onClick={() => { }}
        fontSize='15px'
        color="#333"
        isTextOverflow={true}
      >
        {titleWithoutExt}
      </Link>
      <div>
        {fileExst &&
          <>
            <Text
              className='badge-ext'
              as="span"
              color="#A3A9AE"
              fontSize='15px'
              fontWeight={600}
              title={fileExst}
              truncate={true}
            >
              {fileExst}
            </Text>
            <Icons.FileActionsConvertIcon className='badge' size='small' isfill={true} color='#A3A9AE' />
            <Icons.FileActionsConvertEditDocIcon className='badge' size='small' isfill={true} color='#3B72A7' />
            <Icons.FileActionsLockedIcon className='badge' size='small' isfill={true} color='#3B72A7' />
          </>
        }
      </div>
      <Text
        containerWidth='10%'
        as="div"
        color="#333"
        fontSize='12px'
        fontWeight={600}
        title={fileOwner}
        truncate={true}
      >
        {fileOwner}
      </Text>
      <Link
        containerWidth='12%'
        type='page'
        title={createdDate}
        fontSize='12px'
        fontWeight={400}
        color="#333"
        onClick={() => { }}
        isTextOverflow={true}
      >
        {`Created: ${createdDate}`}
      </Link>
      <Text
        containerWidth='10%'
        as="div"
        color="#333"
        fontSize='12px'
        fontWeight={600}
        title=''
        truncate={true}
      >
        {fileExst
          ? contentLength
          : `Dcs: ${filesCount} / Flds: ${foldersCount}`}
      </Text>
    </SimpleFilesRowContent>
  )
};

export default FilesRowContent;