const getPlugins = async () => {
  const plugins = await (await fetch("http://localhost:3000/plugins")).json();

  return plugins;
};

const activatePlugin = async (id) => {
  const plugin = await (
    await fetch(`http://localhost:3000/plugins/activate/${id}`, {
      method: "PUT",
    })
  ).json();

  return plugin;
};

const uploadPlugin = async (formData) => {
  console.log(formData);

  const plugin = await (
    await fetch("http://localhost:3000/plugins/upload", {
      method: "POST",
      body: formData,
    })
  ).json();

  return plugin;
};

export default { getPlugins, activatePlugin, uploadPlugin };
