using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

public class Dialog
{
    private readonly Game _game;
    private readonly Player _player;
    private TmxObject _currentEntity;
    private bool _showingPrompt;
    private bool _inDialog;
    private TmxMap _map;
    private Texture2D _dialogBox;
    private string[] _dialogPages = {
        "Hello, traveler! Welcome to the suburb.",
        "I hope you enjoy your stay here. Come back anytime!"
    };
    private int _currentPage;
    private int _visibleChars;
    private float _typewriterTimer;
    private const float TYPEWRITER_SPEED = 0.05f;
    private bool _typingComplete;

    private KeyboardState _prevKeyboard;
    private KeyboardState _currKeyboard;

    public Dialog(Game game, Player player)
    {
        _game = game;
        _player = player;
        _map = GameState.CurrentMap;
        _dialogBox = new Texture2D(_game.GraphicsDevice, 1, 1);
        _dialogBox.SetData(new[] { Color.Black * 0.9f });
    }

    public void Update(GameTime gameTime)
    {
        _prevKeyboard = _currKeyboard;
        _currKeyboard = Keyboard.GetState();

        _currentEntity = null;
        _showingPrompt = false;

        foreach (var group in _map.ObjectGroups)
        {
            foreach (var obj in group.Objects)
            {
                if (obj.Name == "npc")
                {
                    var rect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                    if (_player.Bounds.Intersects(rect) && !_inDialog)
                    {
                        _currentEntity = obj;
                        _showingPrompt = true;
                        if (IsKeyPressed(Keys.E))
                        {
                            StartDialog(obj);
                        }
                        break;
                    }
                }
            }
        }

        if (_inDialog)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string text = _dialogPages[_currentPage];
            if (!_typingComplete)
            {
                _typewriterTimer += delta;
                while (_typewriterTimer >= TYPEWRITER_SPEED && _visibleChars < text.Length)
                {
                    _typewriterTimer -= TYPEWRITER_SPEED;
                    _visibleChars++;
                }
                if (_visibleChars >= text.Length) _typingComplete = true;
            }

            if (IsKeyPressed(Keys.E))
            {
                if (!_typingComplete)
                {
                    _visibleChars = text.Length;
                    _typingComplete = true;
                }
                else
                {
                    _currentPage++;
                    if (_currentPage >= _dialogPages.Length)
                    {
                        _inDialog = false;
                        _currentPage = 0;
                    }
                    else
                    {
                        _visibleChars = 0;
                        _typingComplete = false;
                        _typewriterTimer = 0f;
                    }
                }
            }
        }
    }

    private bool IsKeyPressed(Keys key)
    {
        return _currKeyboard.IsKeyDown(key) && !_prevKeyboard.IsKeyDown(key);
    }

    private void StartDialog(TmxObject npc)
    {
        _inDialog = true;
        _currentPage = 0;
        _visibleChars = 0;
        _typingComplete = false;
        _typewriterTimer = 0f;
    }

    public void Draw(SpriteBatch sb, GameTime gt)
    {
        if (_showingPrompt && _currentEntity != null)
        {
            var pos = new Vector2((float)(_currentEntity.X + _currentEntity.Width / 2f), (float)(_currentEntity.Y - 40));
            string text = "(E) Talk";
            var size = GameState.Font.MeasureString(text) * 1.5f;
            sb.DrawString(GameState.Font, text, pos - size * 0.5f, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        }

        if (_inDialog)
        {
            int boxWidth = 600;
            int boxHeight = 100;
            Vector2 boxPos = _player.Position + new Vector2(-boxWidth / 2f + 16, -boxHeight + 160);

            sb.Draw(_dialogBox, new Rectangle((int)boxPos.X, (int)boxPos.Y, boxWidth, boxHeight), Color.White);

            string display = _dialogPages[_currentPage].Substring(0, _visibleChars);
            Vector2 textPos = boxPos + new Vector2(30, 20);
            sb.DrawString(GameState.Font, display, textPos, Color.White);

            if (_typingComplete)
            {
                bool blink = ((int)(gt.TotalGameTime.TotalSeconds * 3)) % 2 == 0;
                if (blink)
                {
                    string txt = "Press E";
                    var sz = GameState.Font.MeasureString(txt) * 1.2f;
                    Vector2 enterPos = boxPos + new Vector2(boxWidth - sz.X - 30, boxHeight - 50);
                    sb.DrawString(GameState.Font, txt, enterPos, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}