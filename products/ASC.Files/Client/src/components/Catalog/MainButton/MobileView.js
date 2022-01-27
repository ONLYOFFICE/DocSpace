import React from 'react';
import styled, { css } from 'styled-components';
import { inject, observer } from 'mobx-react';
import { isMobileOnly } from 'react-device-detect';

import { mobile } from '@appserver/components/utils/device';

import MainButtonMobile from '@appserver/components/main-button-mobile';

const StyledMainButtonMobile = styled(MainButtonMobile)`
  position: fixed;

  right: 21px;
  bottom: 21px;

  @media ${mobile} {
    right: 16px;
    bottom: 16px;
  }

  ${isMobileOnly &&
  css`
    right: 13px;
    bottom: 13px;
  `}
`;

const MobileView = ({
  actionOptions,
  buttonOptions,
  sectionWidth,
  files,
  primaryProgressDataVisible,
  primaryProgressDataPercent,
  primaryProgressDataLoadingFile,
  clearPrimaryProgressData,
  filesToConversion,
  secondaryProgressDataStoreVisible,
  secondaryProgressDataStorePercent,
  secondaryProgressFinished,
}) => {
  const [isOpenButton, setIsOpenButton] = React.useState(false);
  const [percentProgress, setPercentProgress] = React.useState(0);
  const [progressOptions, setProgressOptions] = React.useState([]);

  const [primaryNumEl, setPrimaryNumEl] = React.useState(0);
  const primaryCurrentFile = React.useRef(null);

  const openButtonToggler = () => {
    setIsOpenButton((prevState) => !prevState);
  };

  React.useEffect(() => {
    let currentPrimaryNumEl = primaryNumEl;

    if (primaryCurrentFile.current === null && primaryProgressDataLoadingFile) {
      primaryCurrentFile.current = primaryProgressDataLoadingFile.uniqueId;
      currentPrimaryNumEl = 0;
    }

    if (primaryCurrentFile.current !== null && primaryProgressDataLoadingFile) {
      if (primaryCurrentFile.current !== primaryProgressDataLoadingFile.uniqueId) {
        currentPrimaryNumEl++;
        primaryCurrentFile.current = primaryProgressDataLoadingFile.uniqueId;
      }
    }

    console.log(filesToConversion);

    const newProgressOptions = [
      {
        key: 'primary-progress',
        open: primaryProgressDataVisible,
        label: 'Upload',
        icon: '/static/images/mobile.actions.remove.react.svg',
        percent: primaryProgressDataPercent,
        status: `${primaryProgressDataPercent === 100 ? files.length : currentPrimaryNumEl}/${
          files.length
        }`,
        onCancel: clearPrimaryProgressData,
      },
    ];

    const newPercentProgress = primaryProgressDataPercent;

    if (primaryProgressDataPercent === 100) {
      currentPrimaryNumEl = 0;
      primaryCurrentFile.current = null;
    }

    setPrimaryNumEl(currentPrimaryNumEl);
    setPercentProgress(newPercentProgress);
    setProgressOptions([...newProgressOptions]);
  }, [
    files.length,
    primaryProgressDataVisible,
    primaryProgressDataPercent,
    primaryProgressDataLoadingFile,
    clearPrimaryProgressData,
    filesToConversion,
    secondaryProgressDataStoreVisible,
    secondaryProgressDataStorePercent,
    secondaryProgressFinished,
  ]);

  return (
    <StyledMainButtonMobile
      sectionWidth={sectionWidth}
      actionOptions={actionOptions}
      isOpenButton={isOpenButton}
      onUploadClick={openButtonToggler}
      onClose={openButtonToggler}
      buttonOptions={buttonOptions}
      percent={percentProgress}
      progressOptions={progressOptions}
      title="Upload"
      withButton={true}
    />
  );
};

export default inject(({ uploadDataStore }) => {
  const {
    startUpload,
    converted,
    clearUploadData,
    cancelUpload,
    cancelConversion,
    clearUploadedFiles,
    setUploadPanelVisible,
    files,
    filesToConversion,
    secondaryProgressDataStore,
    primaryProgressDataStore,
  } = uploadDataStore;

  const {
    visible: primaryProgressDataVisible,
    percent: primaryProgressDataPercent,
    loadingFile: primaryProgressDataLoadingFile,
    clearPrimaryProgressData,
  } = primaryProgressDataStore;

  const {
    visible: secondaryProgressDataStoreVisible,
    percent: secondaryProgressDataStorePercent,
    isSecondaryProgressFinished: secondaryProgressFinished,
  } = secondaryProgressDataStore;

  return {
    files,
    primaryProgressDataVisible,
    primaryProgressDataPercent,
    primaryProgressDataLoadingFile,
    clearPrimaryProgressData,
    filesToConversion,
    secondaryProgressDataStoreVisible,
    secondaryProgressDataStorePercent,
    secondaryProgressFinished,
  };
})(observer(MobileView));
