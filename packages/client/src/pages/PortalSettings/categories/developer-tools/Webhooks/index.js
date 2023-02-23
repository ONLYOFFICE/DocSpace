import Button from "@docspace/components/button";
import React, { useState } from "react";
import { CreateWebhookDialog } from "./sub-components/CreateWebhookDialog";
import { Info } from "./sub-components/Info";
import { WebhooksList } from "./sub-components/WebhooksList";

const Webhooks = () => {
  const [isCreateOpen, setIsCreateOpen] = useState(true);
  const onCreateClose = () => {
    setIsCreateOpen(false);
  };
  return (
    <div>
      <Info />
      <Button label="Create webhook" primary size="small" onClick={() => setIsCreateOpen(true)} />
      <WebhooksList />
      <CreateWebhookDialog
        currentColorAccent={null}
        currentColorButtons={null}
        visible={isCreateOpen}
        onClose={onCreateClose}
        header="Create webhook"
      />
    </div>
  );
};

export default Webhooks;
