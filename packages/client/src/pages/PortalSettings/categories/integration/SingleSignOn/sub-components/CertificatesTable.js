import AccessEditReactSvgUrl from "PUBLIC_DIR/images/access.edit.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";
import FileSvgUrl from "PUBLIC_DIR/images/icons/32/file.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import { ContextMenuButton } from "@docspace/components";

import StyledCertificatesTable from "../styled-containers/StyledCertificatesTable";
import { ReactSVG } from "react-svg";

const CertificatesTable = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    prefix,
    setSpCertificate,
    setIdpCertificate,
    delSpCertificate,
    delIdpCertificate,
    idpCertificates,
    spCertificates,
  } = props;

  const renderRow = (certificate, index) => {
    console.log(prefix, index);
    const onEdit = () => {
      prefix === "sp"
        ? setSpCertificate(certificate, index)
        : setIdpCertificate(certificate);
    };

    const onDelete = () => {
      prefix === "sp"
        ? delSpCertificate(certificate.action)
        : delIdpCertificate(certificate.crt);
    };

    const contextOptions = [
      {
        id: "edit",
        key: "edit",
        label: t("Common:EditButton"),
        icon: AccessEditReactSvgUrl,
        onClick: onEdit,
      },
      {
        id: "delete",
        key: "delete",
        label: t("Common:Delete"),
        icon: CatalogTrashReactSvgUrl,
        onClick: onDelete,
      },
    ];

    const getOptions = () => contextOptions;

    const getFullDate = (date) => {
      return `${new Date(date).toLocaleDateString()}`;
    };

    return (
      <div key={`certificate-${index}`} className="row">
        <ReactSVG src={FileSvgUrl} />
        <div className="column">
          <div className="column-row">
            <Text fontWeight={600} fontSize="14px" lineHeight="16px" noSelect>
              {certificate.domainName}
            </Text>
          </div>
          <div className="column-row">
            <Text
              color="#a3a9ae"
              fontSize="12px"
              fontWeight={600}
              lineHeight="16px"
              noSelect
            >
              {certificate.action}
              {" | "}
              {getFullDate(certificate.startDate)}
              {" - "}
              {getFullDate(certificate.expiredDate)}
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
          idpCertificates.map((cert, index) => renderRow(cert, index))}

        {prefix === "sp" &&
          spCertificates.map((cert, index) => renderRow(cert, index))}
      </div>
    </StyledCertificatesTable>
  );
};

export default inject(({ ssoStore }) => {
  const {
    setSpCertificate,
    setIdpCertificate,
    delSpCertificate,
    delIdpCertificate,
    idpCertificates,
    spCertificates,
  } = ssoStore;

  return {
    setSpCertificate,
    setIdpCertificate,
    delSpCertificate,
    delIdpCertificate,
    idpCertificates,
    spCertificates,
  };
})(observer(CertificatesTable));
