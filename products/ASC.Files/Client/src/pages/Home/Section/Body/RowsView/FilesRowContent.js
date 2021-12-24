import React, { useCallback } from 'react';
import { inject, observer } from 'mobx-react';
import { withRouter } from 'react-router';
import { withTranslation } from 'react-i18next';
import styled from 'styled-components';
import { isMobile } from 'react-device-detect';

import Link from '@appserver/components/link';
import Text from '@appserver/components/text';
import RowContent from '@appserver/components/row-content';

import withContent from '../../../../../HOCs/withContent';
import withBadges from '../../../../../HOCs/withBadges';
import { Base } from '@appserver/components/themes';

const SimpleFilesRowContent = styled(RowContent)`
  .row-main-container-wrapper {
    width: 100%;
  }
  .row-content-link {
    color: ${(props) => props.theme.filesSection.linkColor};
  }
  .row-content-text {
    ${(props) => props.theme.filesSection.sideColor}
  }
  .badge-ext {
    margin-right: 8px;
    color: ${(props) => props.theme.filesSection.textColor};
  }

  .badge {
    height: 14px;
    width: 14px;
    margin-right: 6px;
  }
  .lock-file {
    cursor: ${(props) => (props.withAccess ? 'pointer' : 'default')};
  }
  .badges {
    display: flex;
    align-items: center;
    height: 19px;
  }

  .favorite {
    cursor: pointer;
    margin-right: 6px;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

SimpleFilesRowContent.defaultProps = { theme: Base };

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
  fileOwner,
  linkStyles,
  badgesComponent,
  isAdmin,
  theme,
}) => {
  const { contentLength, fileExst, filesCount, foldersCount, providerKey, access, title } = item;

  const withAccess = isAdmin || access === 0;
  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        isFile={fileExst || contentLength}
        sideColor={theme.filesSection.rowView.sideColor}
        withAccess={withAccess}>
        <Link
          className="row-content-link"
          containerWidth="55%"
          type="page"
          title={title}
          fontWeight="600"
          fontSize="15px"
          target="_blank"
          {...linkStyles}
          isTextOverflow={true}>
          {titleWithoutExt}
          {fileExst && (
            <Text className="badge-ext" as="span" fontSize="15px" fontWeight={600} truncate={true}>
              {fileExst}
            </Text>
          )}
        </Link>
        <div className="badges">{badgesComponent}</div>
        <Text
          containerMinWidth="120px"
          containerWidth="15%"
          as="div"
          className="row-content-text"
          fontSize="12px"
          fontWeight={400}
          title={fileOwner}
          truncate={true}>
          {fileOwner}
        </Text>
        <Text
          containerMinWidth="200px"
          containerWidth="15%"
          title={updatedDate}
          fontSize="12px"
          fontWeight={400}
          className="row_update-text row-content-text">
          {updatedDate && updatedDate}
        </Text>
        <Text
          containerMinWidth="90px"
          containerWidth="10%"
          as="div"
          className="row-content-text"
          fontSize="12px"
          fontWeight={400}
          title=""
          truncate={true}>
          {fileExst || contentLength
            ? contentLength
            : !providerKey
            ? `${t('TitleDocuments')}: ${filesCount} | ${t('TitleSubfolders')}: ${foldersCount}`
            : ''}
        </Text>
      </SimpleFilesRowContent>
    </>
  );
};

export default inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withRouter(withTranslation(['Home', 'Translations'])(withContent(withBadges(FilesRowContent)))),
  ),
);
