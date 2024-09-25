using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Header : MarginContainer
{
	private Label _serverVersionLabeL;
	private Label _serverResetLabel;
	private Label _usernameLabel;
	private Label _creditsLabel;

	public override void _Ready()
	{
		_serverVersionLabeL = GetNode<Label>("%GameVersion");
		_serverResetLabel = GetNode<Label>("%NextReset");

		_usernameLabel = GetNode<Label>("%Username");
		_creditsLabel = GetNode<Label>("%Credits");

		Store.Instance.ServerInfoUpdate += OnServerStatusUpdated;
		Store.Instance.AgentInfoUpdate += OnAgentInfoUpdated;
	}

	private void OnServerStatusUpdated()
	{
		var status = Store.Instance.Data.ServerStatus;
		_serverVersionLabeL.Text = "SpaceTraders " + status.Version;
		_serverResetLabel.Text = status.NextReset.ToLocalTime().ToString();
	}

	private void OnAgentInfoUpdated()
	{
		var agent = Store.Instance.Data.Agent;
		_usernameLabel.Text = agent.Name;
		_creditsLabel.Text = FormatCurrency(agent.Credits) + "₡";
	}

	private static string FormatCurrency(long x)
	{
		return $"{x:n0}";
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
