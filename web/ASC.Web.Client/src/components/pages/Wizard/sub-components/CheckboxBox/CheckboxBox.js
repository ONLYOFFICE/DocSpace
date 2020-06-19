import React from 'react';

import {
  Checkbox,
  Link
} from 'asc-web-components';
 
import StyledCheckboxBox from './StyledCheckboxBox';

const CheckboxBox = ({ 
  isSendData, 
  isAcceptLicense,
  sendDataHandler,
  acceptLicenseHandler,
  t
}) => (
  <StyledCheckboxBox>
      <Checkbox
        className="wizard-checkbox"
        id="usage-data"
        name="usage-data"
        label={t('checkboxBox.labelSendData')}
        isChecked={isSendData}
        isIndeterminate={false}
        isDisabled={false}
        onChange={sendDataHandler}
      />
      <Checkbox
        className="wizard-checkbox"
        id="license"
        name="confirm"
        label={t('checkboxBox.labelAcceptLicense')}
        isChecked={isAcceptLicense}
        isIndeterminate={false}
        isDisabled={false}
        onChange={acceptLicenseHandler}
      />
      <Link 
        type="page" 
        color="#116d9d" 
        href="https://gnu.org/licenses/gpl-3.0.html" 
        isBold={false}
      >
        {t('checkboxBox.link')}
      </Link>
    </StyledCheckboxBox>
  )

  export default CheckboxBox;