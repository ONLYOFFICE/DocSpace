
import React from "react";
import styled from "styled-components";
import { RowContent, Link, Text, Icons, Badge } from "asc-web-components";

class FilesRowContent extends React.PureComponent {

  render() {
    const { title, fileExst, created, createdBy, contentLength, filesCount, foldersCount, version, fileStatus } = this.props.item;

    const SimpleFilesRowContent = styled(RowContent)`
    .badge-ext {
      margin-left: -8px;
      margin-right: 8px;
    }

    .badge {
      margin-right: 8px;
    }

    .badges {
      display: flex;
      align-items: center;
    }
  `
    const titleWithoutExt = fileExst
      ? title.split('.').slice(0, -1).join('.')
      : title;

    const fileOwner = (this.props.viewer.id === createdBy.id && "Me") || createdBy.displayName;
    const createdDate = new Date(created).toLocaleString("EN-US");
    const notConverted = ['.pdf', '.zip'];
    const canEdit = fileExst && notConverted.includes(fileExst) ? false : true;

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
            <div className='badges'>
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
              {fileStatus === 4 &&
                <Icons.FileActionsConvertIcon
                  className='badge'
                  size='small'
                  isfill={true}
                  color='#A3A9AE'
                />
              }
              {canEdit &&
                <Icons.AccessEditIcon
                  className='badge'
                  size='small'
                  isfill={true}
                  color='#A3A9AE'
                />
              }
              {fileStatus === 1 &&
                <Icons.FileActionsConvertEditDocIcon
                  className='badge'
                  size='small'
                  isfill={true}
                  color='#3B72A7'
                />
              }
              {false &&
                <Icons.FileActionsLockedIcon
                  className='badge'
                  size='small'
                  isfill={true}
                  color='#3B72A7'
                />
              }
              {version > 1 &&
                <Badge
                  className='badge-version'
                  backgroundColor="#A3A9AE"
                  borderRadius="11px"
                  color="#FFFFFF"
                  fontSize="10px"
                  fontWeight={800}
                  label={`Ver.${version}`}
                  maxWidth="50px"
                  onClick={() => { }}
                  padding="0 5px"
                />
              }
              {fileStatus === 2 &&
                <Badge
                  className='badge-version'
                  backgroundColor="#ED7309"
                  borderRadius="11px"
                  color="#FFFFFF"
                  fontSize="10px"
                  fontWeight={800}
                  label={`New`}
                  maxWidth="50px"
                  onClick={() => { }}
                  padding="0 5px"
                />
              }
            </div>
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
  }
};

export default FilesRowContent;