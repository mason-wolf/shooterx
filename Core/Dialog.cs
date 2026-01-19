using System;
using System.Collections.Generic;
using System.Linq;
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
    private List<string> _dialogPages = new List<string>();
    private int _currentPage;
    private int _visibleChars;
    private float _typewriterTimer;
    private const float TYPEWRITER_SPEED = 0.05f;
    private bool _typingComplete;

    private KeyboardState _prevKeyboard;
    private KeyboardState _currKeyboard;
    private string _entityName;
    private string _interactionText;
    public void AddText(string text)
    {
        _dialogPages.Add(text);
    }

    public void SetInteractionText(string text)
    {
        _interactionText = text;
    }

    public Dialog(Game game, Player player, string entityName)
    {
        _game = game;
        _player = player;
        _map = GameState.CurrentMap;
        _dialogBox = new Texture2D(_game.GraphicsDevice, 1, 1);
        _dialogBox.SetData(new[] { Color.Black * 0.9f });
        this._entityName = entityName;
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
                if (obj.Name == _entityName)
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
            _player.DisableMovement();

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
                    if (_currentPage >= _dialogPages.Count)
                    {
                        _inDialog = false;
                        _currentPage = 0;
                        _player.EnableMovement();
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
        else
        {
            _player.EnableMovement();
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
            var pos = new Vector2((float)_player.Position.X + 80, (float)_player.Position.Y);
            string text = _interactionText ?? "Press E to interact";
            var size = GameState.Font.MeasureString(text) * 1.5f;
            sb.DrawString(GameState.Font, text, pos - size * 0.5f, new Color(100, 255, 100), 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
        }

        if (_inDialog)
        {
            int boxWidth = 150;
            int boxHeight = 25;
            Vector2 boxPos = _player.Position + new Vector2(-50, -25);

            sb.Draw(_dialogBox, new Rectangle((int)boxPos.X, (int)boxPos.Y, boxWidth, boxHeight), Color.White);

            string display = _dialogPages[_currentPage].Substring(0, _visibleChars);
            Vector2 textPos = boxPos + new Vector2(10, 10);
            sb.DrawString(GameState.Font, display, textPos, Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);

            if (_typingComplete)
            {
                bool blink = ((int)(gt.TotalGameTime.TotalSeconds * 3)) % 2 == 0;
                if (blink)
                {
                    string txt = "Press E";
                    var sz = GameState.Font.MeasureString(txt) * 1.2f;
                    Vector2 enterPos = boxPos + new Vector2(boxWidth - sz.X - 30, boxHeight - 30);
                   // sb.DrawString(GameState.Font, txt, enterPos, Color.White, 0f, Vector2.Zero, .2f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}