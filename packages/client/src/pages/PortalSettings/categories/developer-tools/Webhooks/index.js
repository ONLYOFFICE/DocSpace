import Button from '@docspace/components/button';
import React, { useState } from 'react';
import ColorSchemeDialog from './sub-components/colorSchemeDialog';
import { Info } from './sub-components/Info';
import { WebhooksList } from './sub-components/WebhooksList';

const Webhooks = () => {
  const [showColorSchemeDialog, setShowColorSchemeDialog] = useState(true);
  const onClickColor = () => {};
  const onCloseColorSchemeDialog = () => {
    setShowColorSchemeDialog(false);
  };
  return (
    <div>
      <Info />
      <Button
        label="Create webhook"
        primary
        size="small"
        onClick={() => setShowColorSchemeDialog(true)}
      />
      <WebhooksList />
      <ColorSchemeDialog
        onClickColor={onClickColor}
        currentColorAccent={null}
        currentColorButtons={null}
        visible={showColorSchemeDialog}
        onClose={onCloseColorSchemeDialog}
        viewMobile={false}
      />
    </div>
  );
};

export default Webhooks;
