import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

const TableText = ({ t, text }) => (
  <Text truncate className="property-content">
    {t(text)}
  </Text>
);

const TableLink = ({ t, text, href }) => (
  <Link
    isTextOverflow
    className="property-content"
    href={href}
    isHovered={true}
  >
    {t(text)}
  </Link>
);

export default { TableText, TableLink };
