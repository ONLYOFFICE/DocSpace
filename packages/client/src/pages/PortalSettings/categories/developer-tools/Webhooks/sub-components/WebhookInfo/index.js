import React from 'react';

import { Link, InfoText, InfoWrapper } from '../../styled-components';

export const WebhookInfo = () => {
  return (
    <InfoWrapper>
      <InfoText>
        Use webhooks to perform custom actions on the side of any application or website you are
        using based on various events in ONLYOFFICE Docspace. <br />
        Here, you can create and manage all your webhooks, configure them, and browse history of
        every webhook to audit their performance.
      </InfoText>
      <Link href="https://api.onlyoffice.com/portals/basic">Webhooks Guide</Link>
    </InfoWrapper>
  );
};
