import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@appserver/components/text";
import { ContextMenuButton } from "@appserver/components";

import StyledCertificatesTable from "../styled-containers/StyledCertificatesTable";
import { addArguments } from "../../../../utils/addArguments";
import { ReactSVG } from "react-svg";

const CertificatesTable = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    prefix,
    onEditClick,
    onDeleteClick,
    idp_certificates,
    sp_certificates,
  } = props;

  const renderRow = (certificate, index) => {
    const onEdit = addArguments(onEditClick, certificate, prefix);
    const onDelete = addArguments(onDeleteClick, certificate.crt, prefix);

    const contextOptions = [
      {
        key: "edit",
        label: t("Common:EditButton"),
        icon: "static/images/access.edit.react.svg",
        onClick: onEdit,
      },
      {
        key: "delete",
        label: t("Common:Delete"),
        icon: "static/images/catalog.trash.react.svg",
        onClick: onDelete,
      },
    ];

    const getOptions = () => contextOptions;

    const getFullDate = (date) => {
      return `${new Date(date).toLocaleDateString()}`;
    };

    return (
      <div key={`certificate-${index}`} className="row">
        <ReactSVG src="/static/images/icons/24/file.svg" />
        <div className="column">
          <div className="column-row">
            <Text fontWeight={600} fontSize="14px" noSelect>
              {certificate.domainName}
            </Text>
            <Text color="#a3a9ae" fontWeight={600} fontSize="14px" noSelect>
              {", "}
              {getFullDate(certificate.startDate)}
              {" - "}
              {getFullDate(certificate.expiredDate)}
            </Text>
          </div>
          <div className="column-row">
            <Text color="#a3a9ae" fontSize="12px" fontWeight={600} noSelect>
              {certificate.action}
            </Text>
          </div>
        </div>
        <ContextMenuButton
          className="context-btn"
          getData={getOptions}
          usePortal={false}
        />
      </div>
    );
  };

  return (
    <StyledCertificatesTable>
      <div className="body">
        {prefix === "idp" &&
          idp_certificates.map((cert, index) => renderRow(cert, index))}

        {prefix === "sp" &&
          sp_certificates.map((cert, index) => renderRow(cert, index))}
      </div>
    </StyledCertificatesTable>
  );
};

export default inject(({ ssoStore }) => {
  const {
    onEditClick,
    onDeleteClick,
    idp_certificates,
    sp_certificates,
  } = ssoStore;

  return { onEditClick, onDeleteClick, idp_certificates, sp_certificates };
})(observer(CertificatesTable));
