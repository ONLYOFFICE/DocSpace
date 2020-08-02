import React from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { 
  ModalDialog, 
  EmailInput, 
  Button,
  Box,
  utils 
} from 'asc-web-components';

const { tablet } = utils.device;

const BtnContainer = styled(Box)`
  width: 100px;
  
  @media ${tablet} {
    width: 293px;
  }
`;

const BodyContainer = styled(Box)`
  font: 13px 'Open Sans', normal;
  line-height: 20px;
`;

const ModalContainer = ({ 
  t, 
  errorLoading, 
  visibleModal, 
  errorMessage, 
  emailOwner,
  settings,
  onEmailHandler,
  onSaveEmailHandler,
  onCloseModal
}) => {

  let header, content, footer;

  const visible = errorLoading ? errorLoading : visibleModal;

  if(errorLoading) {
    header = t('errorLicenseTitle');
    content = <BodyContainer> 
        {errorMessage ? errorMessage: t('errorLicenseBody')}
    </BodyContainer>;

  } else if( visibleModal ) {
    header = t('changeEmailTitle');

    content = <EmailInput
      tabIndex={1}
      scale={true}
      size='base'
      id="change-email"
      name="email-wizard"
      placeholder={t('placeholderEmail')}
      emailSettings={settings}
      value={emailOwner}
      onValidateInput={onEmailHandler}
    />;

    footer = <BtnContainer>
      <Button
        key="saveBtn"
        label={t('changeEmailBtn')}
        primary={true}
        scale={true}
        size="big"
        onClick={onSaveEmailHandler}
      />
    </BtnContainer>;
  }

  return (
    <ModalDialog
      visible={visible}
      displayType="auto"
      zIndex={310}
      headerContent={header}
      bodyContent={content}
      footerContent={footer}
      onClose={onCloseModal}
    />
  );
}

ModalContainer.propTypes = {
  t: PropTypes.func.isRequired,
  errorLoading: PropTypes.bool.isRequired,
  visibleModal: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  settings: PropTypes.object.isRequired,
  onEmailHandler: PropTypes.func.isRequired,
  onSaveEmailHandler: PropTypes.func.isRequired,
  onCloseModal: PropTypes.func.isRequired
};

export default ModalContainer;