import React from 'react';

import {
  Heading,
  Text
} from 'asc-web-components';

import StyledWelcomeBox from './StyledWelcomeBox';

const WelcomeBox = ({ t }) => (
  <StyledWelcomeBox>
    <Heading level={1} title="Heading form" className="wizard-title">
      {t('welcomeBox.heading')}
    </Heading>

    <Text className="wizard-desc">
    {t('welcomeBox.text')}
    </Text>
  </StyledWelcomeBox>
);

export default WelcomeBox;